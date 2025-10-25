using Domain.Contracts.Responses.Conversation;
using Domain.Entities;
using Domain.Enum.Conversation.Functions;
using System;
using System.Threading.Tasks;

namespace Domain.Interfaces.ServiceInterfaces
{
    public interface IConversationService
    {
        Task<(CreateConversationEnum, Guid?)> CreateConversationAsync(Guid senderId, string receiverUserName);
        Task<Conversation?> GetConversationById(Guid conversationId);
        Task<List<ConversationDto>?> GetAllConversationByUser(Guid userId);
    }
}