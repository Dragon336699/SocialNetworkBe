using Domain.Enum.Conversation.Functions;
using System;
using System.Threading.Tasks;

namespace Domain.Interfaces.ServiceInterfaces
{
    public interface IConversationService
    {
        Task<(CreateConversationEnum, Guid?)> CreateConversationAsync(Guid senderId, string receiverUserName);
        Task<(UpdateConversationEnum, Guid?)> UpdateConversationAsync(Guid conversationId, Guid userId, string? nickName, string? draftMessage);
    }
}