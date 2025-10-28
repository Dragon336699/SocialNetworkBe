using Domain.Contracts.Responses.User;
using Domain.Enum.Conversation.Types;

namespace Domain.Contracts.Responses.ConversationUser
{
    public class ConversationUserDto
    {
        public DateTime JoinedAt { get; set; }
        public string? NickName { get; set; }
        public ConversationRole RoleName { get; set; }
        public string? DraftMessage { get; set; }
        public UserDto? User { get; set; }
    }
}
