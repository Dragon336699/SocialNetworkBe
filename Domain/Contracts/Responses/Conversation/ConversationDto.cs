using Domain.Contracts.Responses.ConversationUser;
using Domain.Contracts.Responses.Message;
using Domain.Entities;
using Domain.Enum.Conversation.Types;

namespace Domain.Contracts.Responses.Conversation
{
    public class ConversationDto
    {
        public Guid Id { get; set; }
        public ConversationType Type { get; set; }
        public string? ConversationName { get; set; }
        public List<ConversationUserDto>? ConversationUsers { get; set; }
        public MessageDto? NewestMessage { get; set; } = null;
    }
}
