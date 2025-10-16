using Domain.Entities;
using Domain.Enum.Conversation.Types;
using Domain.Interfaces.ServiceInterfaces;
using Domain.Interfaces.UnitOfWorkInterface;
using SocialNetworkBe.Services.UserServices;

namespace SocialNetworkBe.Services.ConversationUserServices
{
    public class ConversationUserService : IConversationUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ConversationUserService> _logger;
        private readonly IUserService _userService;
        public ConversationUserService (
            IUnitOfWork unitOfWork, 
            ILogger<ConversationUserService> logger, 
            IUserService userService
            )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userService = userService;
        }

        public async Task<Guid?> CheckExist(Guid senderId, Guid receiverId)
        {
            var conversationId = await _unitOfWork.ConversationUserRepository.GetConversationIdBetweenUserAsync(senderId, receiverId);
            return conversationId == null ? null : conversationId;
        }

        public async Task AddUsersToConversationAsync(Guid conversationId, Guid senderId, Guid receiverId)
        {
            try
            {
                var senderInfo = await _userService.GetUserInfoByUserId(senderId.ToString());
                var receiverInfo = await _userService.GetUserInfoByUserId(receiverId.ToString());

                var conversationUsers = new List<ConversationUser>
                {
                    new ConversationUser {
                        ConversationId = conversationId,
                        UserId = senderId,
                        JoinedAt = DateTime.Now,
                        RoleName = ConversationRole.User,
                        NickName = senderInfo?.UserName,
                        DraftMessage = null
                    },

                    new ConversationUser {
                        ConversationId = conversationId,
                        UserId = receiverId,
                        JoinedAt = DateTime.Now,
                        RoleName = ConversationRole.User,
                        NickName = receiverInfo?.UserName,
                        DraftMessage = null
                    }
                };              
                _unitOfWork.ConversationUserRepository.AddRange(conversationUsers);

                int rowsAffected = await _unitOfWork.CompleteAsync();
                if (rowsAffected == 0)
                {
                    throw new Exception("Failed to add users to conversation.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding users to conversation");
                throw;
            }
        }
    }
}
