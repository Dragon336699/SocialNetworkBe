using Domain.Interfaces.ServiceInterfaces;
using Domain.Interfaces.UnitOfWorkInterface;

namespace SocialNetworkBe.Services.ConversationUserServices
{
    public class ConversationUserService : IConversationUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ConversationUserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid?> CheckExist(Guid senderId, Guid receiverId)
        {
            var conversationId = await _unitOfWork.ConversationUserRepository.GetConversationIdBetweenUserAsync(senderId, receiverId);
            return conversationId == null ? null : conversationId;
        }
    }
}
