using Domain.Entities;

namespace Domain.Contracts.Responses.Message
{
    public class SendMessageResponse
    {
        public required bool Status { get; set; }
        public required string Message { get; set; }
        public MessageDto? NewMessage { get; set; }

    }
}
