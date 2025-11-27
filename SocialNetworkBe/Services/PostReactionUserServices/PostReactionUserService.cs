using Domain.Entities;
using Domain.Enum.Post.Functions;
using Domain.Interfaces.ServiceInterfaces;
using Domain.Interfaces.UnitOfWorkInterface;
using SocialNetworkBe.Services.PostServices;

namespace SocialNetworkBe.Services.PostReactionServices
{
    public class PostReactionUserService : IPostReactionUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PostReactionUserService> _logger;
        public PostReactionUserService(IUnitOfWork unitOfWork, ILogger<PostReactionUserService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<PostReactionUser>> GetPostReactionUsersByPostId(Guid postId)
        {
            try
            {
                IEnumerable<PostReactionUser>? postReactionUsers = await _unitOfWork.PostReactionUserRepository.GetAllAsync();
                if (postReactionUsers == null) return Enumerable.Empty<PostReactionUser>();
                return postReactionUsers;
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting post reaction {PostId}", postId);
                throw;
            }
        }
    }
}
