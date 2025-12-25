namespace Domain.Contracts.Responses.User
{
    public class SuggestUserDto
    {
        public Guid target_user_id { get; set; }
        public double probs {  get; set; }
    } 
}
