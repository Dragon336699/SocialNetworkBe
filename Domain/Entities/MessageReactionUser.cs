namespace Domain.Entities
{
    public class MessageReactionUser
    {
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public Guid MessageId { get; set; }
        public Message? Message { get; set; }
        public Guid ReactionId { get; set; }
        public Reaction? Reaction { get; set; }
    }
}
