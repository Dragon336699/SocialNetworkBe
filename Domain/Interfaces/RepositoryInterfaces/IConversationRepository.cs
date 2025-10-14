using Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Domain.Interfaces.RepositoryInterfaces
{
    public interface IConversationRepository : IGenericRepository<Conversation>
    {
        // Kiểm tra cuộc hội thoại tồn tại giữa hai người dùng
        Task<Conversation?> GetConversationBetweenUsersAsync(Guid user1Id, Guid user2Id);

        // Thêm hoặc cập nhật Message
        Task<Message> AddOrUpdateMessageAsync(Message message);
    }
}