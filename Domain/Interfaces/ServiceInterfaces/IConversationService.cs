using Domain.Contracts.Responses.Conversation;
using Domain.Entities;
using Domain.Enum.Conversation.Functions;
using Domain.Enum.Conversation.Types;
using System;
using System.Threading.Tasks;

namespace Domain.Interfaces.ServiceInterfaces
{
    public interface IConversationService
    {
        Task<(CreateConversationEnum, Guid?)> CreateConversationAsync(ConversationType conversationType, List<Guid> userIds);
        Task<Conversation?> GetConversationById(Guid conversationId);
        Task<List<ConversationDto>?> GetAllConversationByUser(Guid userId);
        Task<ConversationDto?> GetConversationForList(Guid conversationId, Guid userId);
    }
}