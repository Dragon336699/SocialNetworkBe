using Domain.Entities;
using Domain.Enum.Conversation.Functions;
using Domain.Enum.Conversation.Types;
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
        private readonly ILogger<ConversationService> _logger;

        public ConversationService(
            IUnitOfWork unitOfWork,
            IConversationUserService conversationUserService,
            IUserService userService,
            ILogger<ConversationService> logger)
        {
            _unitOfWork = unitOfWork;
            _conversationUserService = conversationUserService;
            _userService = userService;
            _logger = logger;
        }

        public async Task<(CreateConversationEnum, Guid?)> CreateConversationAsync(Guid senderId, string receiverUserName)
        {
            try
            {              
                
                var (userFound, receiver) = await _userService.GetUserInfoByUserName(receiverUserName);
                if (!userFound || receiver == null)
                {
                    return (CreateConversationEnum.ReceiverNotFound, null);
                }
             
                Guid? existingConversationId = await _conversationUserService.CheckExist(senderId, receiver.Id);
                if (existingConversationId != null)
                {
                    return (CreateConversationEnum.ConversationExists, existingConversationId);
                }
               
                var conversation = new Conversation
                {
                    Id = Guid.NewGuid(),
                    Type = ConversationType.Personal,
                    CreatedAt = DateTime.Now,
                };
                _unitOfWork.ConversationRepository.Add(conversation);

                var userIds = new List<Guid> { senderId, receiver.Id };              
                await _conversationUserService.AddUsersToConversationAsync(conversation.Id, userIds);

                return (CreateConversationEnum.CreateConversationSuccess, conversation.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating conversation");
                return (CreateConversationEnum.CreateConversationFailed, null);
            }
        }

        public async Task<(UpdateConversationEnum, Guid?)> UpdateConversationAsync(Guid conversationId, Guid userId, string? nickName, string? draftMessage)
        {
            try
            {
                var conversation = await _unitOfWork.ConversationRepository.GetByIdAsync(conversationId);
                if (conversation == null)
                {
                    return (UpdateConversationEnum.ConversationNotFound, null);
                }

                var conversationUser = await _unitOfWork.ConversationUserRepository.FindFirstAsync(cu => cu.ConversationId == conversationId && cu.UserId == userId);
                if (conversationUser == null)
                {
                    return (UpdateConversationEnum.ConversationUserNotFound, null);
                }

                var changed = false;

                if (nickName != null && conversationUser.NickName != nickName)
                {
                    conversationUser.NickName = nickName;
                    changed = true;
                }

                if (draftMessage != null && conversationUser.DraftMessage != draftMessage)
                {
                    conversationUser.DraftMessage = draftMessage;
                    changed = true;
                }

                if (changed)
                {
                    _unitOfWork.ConversationUserRepository.Update(conversationUser);
                    var rows = await _unitOfWork.CompleteAsync();
                    if (rows == 0)
                        return (UpdateConversationEnum.UpdateConversationFailed, null);
                }

                return (UpdateConversationEnum.UpdateConversationSuccess, conversationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating conversation");
                return (UpdateConversationEnum.UpdateConversationFailed, null);
            }
        }
    }
}