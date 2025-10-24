using Domain.Entities;
using Domain.Enum.Conversation.Types;
using Domain.Interfaces.ServiceInterfaces;
using Domain.Interfaces.UnitOfWorkInterface;
using SocialNetworkBe.Services.UserServices;
using Domain.Contracts.Responses.User;
using Domain.Contracts.Requests.ConversationUser;
namespace SocialNetworkBe.Services.ConversationUserServices
{
    public class ConversationUserService : IConversationUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ConversationUserService> _logger;
        private readonly IUserService _userService;
        public ConversationUserService(
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
            var conversationId = await _unitOfWork.ConversationUserRepository.GetConversationIdBetweenUsersAsync(senderId, receiverId);
            return conversationId == null ? null : conversationId;
        }

        public async Task AddUsersToConversationAsync(Guid conversationId, List<Guid> userIds)
        {
            try
            {
                var conversationUsers = new List<ConversationUser>();

                foreach (var userId in userIds)
                {
                    UserDto? userInfo = await _userService.GetUserInfoByUserId(userId.ToString());
                    if (userInfo == null)
                    {
                        _logger.LogWarning($"User with ID {userId} not found.");
                        return;
                    }
                    var conversationUser = new ConversationUser
                    {
                        ConversationId = conversationId,
                        UserId = userId,
                        JoinedAt = DateTime.Now,
                        RoleName = ConversationRole.User,
                        NickName = userInfo.UserName,
                        DraftMessage = null
                    };

                    conversationUsers.Add(conversationUser);
                }

                if (!conversationUsers.Any())
                {
                    throw new Exception("No valid users to add to the conversation.");
                }

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

        public async Task<IEnumerable<ConversationUser>?> GetConversationUser(GetConversationUserRequest request)
        {
            try
            {
                IEnumerable<ConversationUser>? conversationUser = await _unitOfWork.ConversationUserRepository.FindAsyncWithIncludes(cu => cu .ConversationId == request.ConversationId, cu => cu.User, cu => cu.Conversation);
                if (conversationUser == null) return null;
                return conversationUser;
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting conversation user");
                throw;
            }
        }
    }
}
