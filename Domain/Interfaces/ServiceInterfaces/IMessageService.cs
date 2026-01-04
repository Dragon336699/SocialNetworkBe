using Domain.Contracts.Requests.Message;
using Domain.Contracts.Responses.Message;
using Domain.Entities;
using Domain.Enum.Message.Functions;
using Domain.Enum.Message.Types;

namespace Domain.Interfaces.ServiceInterfaces
{
    public interface IMessageService
    {
        Task<(GetMessagesEnum, List<MessageDto>?)> GetMessages(GetMessagesRequest request);
        Task<MessageDto?> SaveMessage(SendMessageRequest request);
        Task<bool> UpdateMessage(Guid messageId, MessageStatus status);
        Task<MessageDto?> GetMessageById(Guid messageId);
        Task<(SendMessageEnum, MessageDto?)> SendMessage(SendMessageRequest request);
        Task<MessageDto?> AddUpdateDeleteReactionMessage(ReactionMessageRequest request, Guid userId);
        Task<int> GetUnreadMessagesNumber(Guid userId);
        Task<List<MessageAttachment>?> GetImageAttachmentsByConversationId(Guid conversationId, int skip, int take);
    }
}
