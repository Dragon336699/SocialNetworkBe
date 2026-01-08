using Domain.Interfaces.RepositoryInterfaces;
using Domain.Interfaces.ServiceInterfaces;

namespace SocialNetworkBe.Services.InteractionServices
{
    public class InteractionService : IInteractionService
    {
        private readonly ILogger<InteractionService> _logger;
        private readonly IInteractionRepository _interactionRepository;
        public InteractionService(ILogger<InteractionService> logger, IInteractionRepository interactionRepository)
        {
            _logger = logger;
            _interactionRepository = interactionRepository;
        }

        public void IncreaseSearchCount (Guid userId, Guid targetUserId)
        {
            try
            {
                _interactionRepository.IncreaseSearchCount (userId, targetUserId);
            } catch (Exception ex)
            {
                _logger.LogError("An error occur while increasing search count with user {userId} and {targetUserId}", userId, targetUserId);
                throw;
            }
        }

        public void IncreaseViewCount(Guid userId, Guid targetUserId)
        {
            try
            {
                _interactionRepository.IncreaseViewCount(userId, targetUserId);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occur while increasing search count with user {userId} and {targetUserId}", userId, targetUserId);
                throw;
            }
        }

        public void IncreaseLikeCount(Guid userId, Guid targetUserId)
        {
            try
            {
                _interactionRepository.IncreaseLikeCount(userId, targetUserId);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occur while increasing search count with user {userId} and {targetUserId}", userId, targetUserId);
                throw;
            }
        }

        public void InteractionPost(Guid userId, Guid postId, string action)
        {
            try
            {
                _interactionRepository.InteractionPost(userId, postId, action);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occur while interaction post with user {userId} {action} post {postId}", userId, action, postId);
                throw;
            }
        }
    }
}
