using Domain.Contracts.Responses.Conversation;
using Domain.Entities;

namespace Domain.Interfaces.RepositoryInterfaces
{
    public interface IConversationRepository : IGenericRepository<Conversation>
    {
        public Task<List<ConversationDto>?> GetAllConversationByUser(Guid userId);
    }
}