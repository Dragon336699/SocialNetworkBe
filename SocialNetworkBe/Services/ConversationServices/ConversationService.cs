using AutoMapper;
using Domain.Contracts.Responses.Conversation;
using Domain.Entities;
using Domain.Enum.Conversation.Functions;
using Domain.Enum.Conversation.Types;
using Domain.Enum.User.Types;
using Domain.Interfaces.ServiceInterfaces;
using Domain.Interfaces.UnitOfWorkInterface;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace SocialNetworkBe.Services.ConversationServices
{
    public class ConversationService : IConversationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConversationUserService _conversationUserService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ILogger<ConversationService> _logger;

        public ConversationService(
            IUnitOfWork unitOfWork,
            IConversationUserService conversationUserService,
            IUserService userService,
            IMapper mapper,
            ILogger<ConversationService> logger)
        {
            _unitOfWork = unitOfWork;
            _conversationUserService = conversationUserService;
            _userService = userService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<(CreateConversationEnum, Guid?)> CreateConversationAsync(ConversationType conversationType, List<Guid> userIds)
        {
            try
            {
                var names = new List<string>();
                int count = 0;
                foreach (var userId in userIds)
                {
                    var userInfo = await _userService.GetUserInfoByUserId(userId.ToString());
                    if (userInfo == null)
                    {
                        return (CreateConversationEnum.ReceiverNotFound, null);
                    }

                    if (count < 3)
                    {
                        names.Add(userInfo.FirstName);
                    }
                    count++;
                }

                var conversationName = string.Join(", ", names);

                if (conversationType == ConversationType.Personal && userIds.Count == 2)
                {
                    Guid? existingConversationId = await _conversationUserService.CheckExist(userIds[0], userIds[1]);
                    if (existingConversationId != null)
                    {
                        return (CreateConversationEnum.ConversationExists, existingConversationId);
                    }
                }
                var conversation = new Conversation
                {
                    Id = Guid.NewGuid(),
                    Type = conversationType,
                    ConversationName = userIds.Count > 2 ? conversationName : null,
                    CreatedAt = DateTime.UtcNow,
                };
                _unitOfWork.ConversationRepository.Add(conversation);

                await _conversationUserService.AddUsersToConversationAsync(conversation.Id, userIds);

                return (CreateConversationEnum.CreateConversationSuccess, conversation.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating conversation");
                return (CreateConversationEnum.CreateConversationFailed, null);
            }
        }

        public async Task<Conversation?> GetConversationById(Guid conversationId)
        {
            try
            {
                Conversation? conversation = await _unitOfWork.ConversationRepository.GetByIdAsync(conversationId);
                return conversation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting conversation");
                throw;
            }
        }

        public async Task<List<ConversationDto>?> GetAllConversationByUser(Guid userId)
        {
            try
            {
                var conversationUser = await _unitOfWork.ConversationUserRepository.FindAsync(cu => cu.UserId == userId);
                if (conversationUser == null) return null;
                List<ConversationDto>? conversations = await _unitOfWork.ConversationRepository.GetAllConversationByUser(userId);

                if (conversations == null || !conversations.Any()) return null;
            
                var blockedUserIds = await GetBlockedUserIdsAsync(userId);

                if (blockedUserIds.Any())
                {                  
                    conversations = conversations.Where(conv =>
                    {                      
                        if (conv.Type == ConversationType.Group)
                            return true;

                        if (conv.Type == ConversationType.Personal)
                        {
                            var otherUsers = conv.ConversationUsers?
                                .Where(cu => cu.User != null && cu.User.Id != userId)
                                .Select(cu => cu.User!.Id)
                                .ToList() ?? new List<Guid>();
                          
                            return !otherUsers.Any(uid => blockedUserIds.Contains(uid));
                        }
                        return true;
                    }).ToList();                                
                }
                return conversations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all conversations");
                throw;
            }
        }

        public async Task<ConversationDto?> GetConversationForList(Guid conversationId, Guid userId)
        {
            try
            {
                var conversationUser = await _unitOfWork.ConversationUserRepository.FindAsync(cu => cu.UserId == userId);
                if (conversationUser == null) return null;
                ConversationDto? conversation = await _unitOfWork.ConversationRepository.GetConversationForList(conversationId, userId);
                return conversation;
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting conversation for list");
                throw;
            }
        }

        public async Task<DeleteConversationEnum> DeleteConversationAsync(Guid conversationId, Guid userId)
        {
            try
            {
                var conversation = await _unitOfWork.ConversationRepository.GetByIdAsync(conversationId);
                if (conversation == null)
                {
                    return DeleteConversationEnum.ConversationNotFound;
                }

                var conversationUser = await _unitOfWork.ConversationUserRepository
                    .FindAsync(cu => cu.ConversationId == conversationId && cu.UserId == userId);

                if (conversationUser == null || !conversationUser.Any())
                {
                    return DeleteConversationEnum.UserNotInConversation;
                }

                var messages = await _unitOfWork.MessageRepository
                    .FindAsync(m => m.ConversationId == conversationId);
                if (messages != null && messages.Any())
                {
                    _unitOfWork.MessageRepository.RemoveRange(messages);
                }

                var allConversationUsers = await _unitOfWork.ConversationUserRepository
                    .FindAsync(cu => cu.ConversationId == conversationId);
                if (allConversationUsers != null && allConversationUsers.Any())
                {
                    _unitOfWork.ConversationUserRepository.RemoveRange(allConversationUsers);
                }

                _unitOfWork.ConversationRepository.Remove(conversation);
               
                int rowsAffected = await _unitOfWork.CompleteAsync();
                if (rowsAffected == 0)
                {
                    return DeleteConversationEnum.Failed;
                }

                return DeleteConversationEnum.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting conversation");
                return DeleteConversationEnum.Failed;
            }
        }

        public async Task<ChangeNicknameEnum> ChangeNicknameAsync(Guid conversationId, Guid currentUserId, Guid targetUserId, string newNickname)
        {
            try
            {
                var conversation = await _unitOfWork.ConversationRepository.GetByIdAsync(conversationId);
                if (conversation == null)
                {
                    return ChangeNicknameEnum.ConversationNotFound;
                }

                var currentUserInConversation = await _unitOfWork.ConversationUserRepository
                    .FindAsync(cu => cu.ConversationId == conversationId && cu.UserId == currentUserId);

                if (currentUserInConversation == null || !currentUserInConversation.Any())
                {
                    return ChangeNicknameEnum.UserNotInConversation;
                }

                var targetConversationUser = await _unitOfWork.ConversationUserRepository
                    .FindAsync(cu => cu.ConversationId == conversationId && cu.UserId == targetUserId);

                if (targetConversationUser == null || !targetConversationUser.Any())
                {
                    return ChangeNicknameEnum.UserNotInConversation;
                }

                var conversationUserToUpdate = targetConversationUser.First();
                conversationUserToUpdate.NickName = newNickname;
                _unitOfWork.ConversationUserRepository.Update(conversationUserToUpdate);
             
                int rowsAffected = await _unitOfWork.CompleteAsync();
                if (rowsAffected == 0)
                {
                    return ChangeNicknameEnum.Failed;
                }

                return ChangeNicknameEnum.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while changing nickname");
                return ChangeNicknameEnum.Failed;
            }
        }
     
        public async Task<ChangeConversationNameEnum> ChangeConversationNameAsync(Guid conversationId, Guid userId, string newConversationName)
        {
            try
            {               
                var conversation = await _unitOfWork.ConversationRepository.GetByIdAsync(conversationId);
                if (conversation == null)
                {
                    return ChangeConversationNameEnum.ConversationNotFound;
                }
              
                if (conversation.Type != ConversationType.Group)
                {
                    return ChangeConversationNameEnum.NotGroupConversation;
                }
             
                var conversationUser = await _unitOfWork.ConversationUserRepository
                    .FindAsync(cu => cu.ConversationId == conversationId && cu.UserId == userId);

                if (conversationUser == null || !conversationUser.Any())
                {
                    return ChangeConversationNameEnum.UserNotInConversation;
                }
            
                conversation.ConversationName = newConversationName;
                _unitOfWork.ConversationRepository.Update(conversation);
           
                int rowsAffected = await _unitOfWork.CompleteAsync();
                if (rowsAffected == 0)
                {
                    return ChangeConversationNameEnum.Failed;
                }

                return ChangeConversationNameEnum.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while changing conversation name");
                return ChangeConversationNameEnum.Failed;
            }
        }

        private async Task<List<Guid>> GetBlockedUserIdsAsync(Guid userId)
        {
            try
            {
                var blockedByMe = await _unitOfWork.UserRelationRepository.FindAsync(
                    ur => ur.UserId == userId && ur.RelationType == UserRelationType.Blocked
                );

                var blockedMe = await _unitOfWork.UserRelationRepository.FindAsync(
                    ur => ur.RelatedUserId == userId && ur.RelationType == UserRelationType.Blocked
                );

                var blockedIds = new List<Guid>();

                if (blockedByMe != null && blockedByMe.Any())
                {
                    blockedIds.AddRange(blockedByMe.Select(ur => ur.RelatedUserId));
                }

                if (blockedMe != null && blockedMe.Any())
                {
                    blockedIds.AddRange(blockedMe.Select(ur => ur.UserId));
                }

                return blockedIds.Distinct().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting blocked users for user {UserId}", userId);
                return new List<Guid>();
            }
        }
    }
}