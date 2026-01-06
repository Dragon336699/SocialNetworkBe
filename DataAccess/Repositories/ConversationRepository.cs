using DataAccess.DbContext;
using Domain.Contracts.Responses.Conversation;
using Domain.Contracts.Responses.ConversationUser;
using Domain.Entities;
using Domain.Enum.Conversation.Types;
using Domain.Interfaces.RepositoryInterfaces;
using Domain.Contracts.Responses.User;
using Microsoft.EntityFrameworkCore;
using Domain.Contracts.Responses.Message;

namespace DataAccess.Repositories
{
    public class ConversationRepository : GenericRepository<Conversation>, IConversationRepository
    {
        public ConversationRepository(SocialNetworkDbContext context) : base(context)
        {
        }

        public async Task<List<ConversationDto>?> GetAllConversationByUser(Guid userId)
        {
            // Lấy danh dách conversations
            var conversations = await _context.Conversation
                                    .Where(c => c.ConversationUsers.Any(cu => cu.UserId == userId))
                                    .Select(c => new ConversationDto
                                    {
                                        Id = c.Id,
                                        Type = c.Type,
                                        ConversationName = c.ConversationName
                                    }).AsNoTracking().ToListAsync();
            // Danh sách conversationId
            var conversationIds = conversations.Select(c => c.Id).ToList();

            // Load users cho các conversation
            var conversationUsers = await _context.ConversationUser
                                    .Where(cu => conversationIds.Contains(cu.ConversationId))
                                    .Include(cu => cu.User)
                                    .AsNoTracking()
                                    .ToListAsync();

            var newestMessageIds = await _context.Message
                    .Where(m => conversationIds.Contains(m.ConversationId))
                    .GroupBy(m => m.ConversationId) // Gom các message theo conversationId
                    .Select(g => g.OrderByDescending(m => m.CreatedAt).Select(m => m.Id).FirstOrDefault())
                    .ToListAsync();

            // Lấy newest message cho từng conversation
            var newestMessages = await _context.Message
                    .Where(m => newestMessageIds.Contains(m.Id))
                    .Include(m => m.Sender)
                    .AsNoTracking()
                    .Select(m => new MessageDto
                    {
                        Id = m.Id,
                        Content = m.Content,
                        Status = m.Status.ToString(),
                        CreatedAt = m.CreatedAt,
                        SenderId = m.Sender.Id,
                        ConversationId = m.ConversationId,
                        Sender = new UserDto
                        {
                            Id = m.Sender.Id,
                            AvatarUrl = m.Sender.AvatarUrl,
                            UserName = m.Sender.UserName,
                            Email = m.Sender.Email,
                            FirstName = m.Sender.FirstName,
                            Status = m.Sender.Status.ToString()
                        }
                    })
                    .ToListAsync();

            foreach (var c in conversations)
            {
                var usersQuery = conversationUsers
                    .Where(cu => cu.ConversationId == c.Id);

                // Logic khác nhau theo type
                if (c.Type == ConversationType.Personal)
                {
                    usersQuery = usersQuery.Where(cu => cu.UserId != userId);
                }

                var users = usersQuery
                    .OrderBy(cu => cu.JoinedAt)
                    .Select(cu => new ConversationUserDto
                    {
                        JoinedAt = cu.JoinedAt,
                        NickName = cu.NickName,
                        RoleName = cu.RoleName,
                        DraftMessage = cu.DraftMessage,
                        User = new UserDto
                        {
                            Id = cu.User.Id,
                            UserName = cu.User.UserName,
                            AvatarUrl = cu.User.AvatarUrl,
                            Email = cu.User.Email,
                            Status = cu.User.Status.ToString(),
                            FirstName = cu.User.FirstName,
                            LastName = cu.User.LastName
                        }
                    })
                    .ToList();

                // Take chỉ cho group
                c.ConversationUsers = c.Type == ConversationType.Group ? users.Take(2).ToList() : users;

                // Newest message
                var newest = newestMessages.FirstOrDefault(m => m.ConversationId == c.Id);
                if (newest != null)
                {
                    c.NewestMessage = newest;
                }
            }

            // Sắp xếp conversations theo tin nhắn mới nhất
            conversations = conversations
                .OrderByDescending(c => c.NewestMessage?.CreatedAt ?? DateTime.MinValue)
                .ToList();

            return conversations;
        }

        public async Task<ConversationDto?> GetConversationForList(Guid conversationId, Guid userId)
        {
            var conversation = await _context.Conversation
                                    .Where(c => c.ConversationUsers.Any(cu => cu.UserId == userId) && c.Id == conversationId)
                                    .Select(c => new ConversationDto
                                    {
                                        Id = c.Id,
                                        Type = c.Type,
                                        ConversationName = c.ConversationName,
                                        ConversationUsers = c.Type == ConversationType.Personal
                                            ? c.ConversationUsers
                                              .Where(cu => cu.UserId != userId)
                                              .Select(cu => new ConversationUserDto
                                              {
                                                  JoinedAt = cu.JoinedAt,
                                                  NickName = cu.NickName,
                                                  RoleName = cu.RoleName,
                                                  DraftMessage = cu.DraftMessage,
                                                  User = new UserDto
                                                  {
                                                      Id = cu.UserId,
                                                      AvatarUrl = cu.User.AvatarUrl,
                                                      Email = cu.User.Email,
                                                      UserName = cu.User.UserName,
                                                      Status = cu.User.Status.ToString(),
                                                      FirstName = cu.User.FirstName,
                                                      LastName = cu.User.LastName,
                                                  }
                                              })
                                              .ToList()
                                            : c.ConversationUsers
                                              .OrderBy(cu => cu.JoinedAt)
                                              .Select(cu => new ConversationUserDto
                                              {
                                                  JoinedAt = cu.JoinedAt,
                                                  NickName = cu.NickName,
                                                  RoleName = cu.RoleName,
                                                  DraftMessage = cu.DraftMessage,
                                                  User = new UserDto
                                                  {
                                                      Id = cu.UserId,
                                                      AvatarUrl = cu.User.AvatarUrl,
                                                      Email = cu.User.Email,
                                                      UserName = cu.User.UserName,
                                                      Status = cu.User.Status.ToString(),
                                                      FirstName = cu.User.FirstName,
                                                      LastName = cu.User.LastName,
                                                  }
                                              })
                                              .Take(2)
                                              .ToList(),
                                        NewestMessage = c.Messages.OrderByDescending(m => m.CreatedAt).Select(m => new MessageDto
                                        {
                                            Id = m.Id,
                                            Content = m.Content,
                                            Status = m.Status.ToString(),
                                            CreatedAt = m.CreatedAt,
                                            UpdatedAt = m.UpdatedAt,
                                            SenderId = m.SenderId,
                                            ConversationId = m.ConversationId,
                                            MessageAttachments = m.MessageAttachments.ToList(),
                                            Sender = new UserDto
                                            {
                                                Id = m.SenderId,
                                                AvatarUrl = m.Sender.AvatarUrl,
                                                Email = m.Sender.Email,
                                                UserName = m.Sender.UserName,
                                                Status = m.Sender.Status.ToString(),
                                                FirstName = m.Sender.FirstName,
                                                LastName = m.Sender.LastName,
                                            }
                                        }).FirstOrDefault()
                                    }).AsNoTracking().FirstOrDefaultAsync();
            return conversation;
        }
    }
}