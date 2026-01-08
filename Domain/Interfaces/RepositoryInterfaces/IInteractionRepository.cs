namespace Domain.Interfaces.RepositoryInterfaces
{
    public interface IInteractionRepository
    {
        public void IncreaseSearchCount(Guid userId, Guid targetUserId);
        public void IncreaseViewCount(Guid userId, Guid targetUserId);
        public void IncreaseLikeCount(Guid userId, Guid targetUserId);
        public void InteractionPost(Guid userId, Guid postId, string action);
    }
}
