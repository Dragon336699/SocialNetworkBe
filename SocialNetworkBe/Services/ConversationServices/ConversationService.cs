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
                // Kiểm tra receiver tồn tại
                var (userFound, receiver) = await _userService.GetUserInfoByUserName(receiverUserName);
                if (!userFound || receiver == null)
                {
                    return (CreateConversationEnum.ReceiverNotFound, null);
                }

                // Kiểm tra conversation tồn tại
                Guid? existingConversationId = await _conversationUserService.CheckExist(senderId, receiver.Id);
                if (existingConversationId != null)
                {
                    return (CreateConversationEnum.ConversationExists, existingConversationId);
                }

                // Tạo Conversation mới
                var conversation = new Conversation
                {
                    Id = Guid.NewGuid(),
                    Type = ConversationType.Personal,
                    CreatedAt = DateTime.Now,
                };

                _unitOfWork.ConversationRepository.Add(conversation);

                // Thêm sender và receiver vào conversation
                await _conversationUserService.AddUsersToConversationAsync(conversation.Id, senderId, receiver.Id);

                return (CreateConversationEnum.CreateConversationSuccess, conversation.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating conversation");
                return (CreateConversationEnum.CreateConversationFailed, null);
            }
        }
    }
}