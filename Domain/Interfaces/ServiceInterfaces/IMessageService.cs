using Domain.Contracts.Requests.Message;
using Domain.Contracts.Responses.Message;
using Domain.Entities;
using Domain.Enum.Message.Functions;

namespace Domain.Interfaces.ServiceInterfaces
{
    public interface IMessageService
    {
        Task<(GetMessagesEnum, List<MessageDto>?)> GetMessages(GetMessagesRequest request);
        MessageDto? SaveMessage(SendMessageRequest request, Guid conversationId, Guid receiverId);
    }
}
