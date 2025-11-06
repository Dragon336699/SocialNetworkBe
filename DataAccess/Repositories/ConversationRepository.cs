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
            var conversation = await _context.Conversation
                                    .Where(c => c.ConversationUsers.Any(cu => cu.UserId == userId))
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
                                              .Select( cu => new ConversationUserDto
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
                                    }).AsNoTracking().ToListAsync();
            return conversation;
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