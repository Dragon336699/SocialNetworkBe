namespace Domain.Contracts.Responses.User
{
    public class SuggestUserDataDto
    {
        public Guid user_id { get; set; }
        public List<SuggestUserDto> recommendations { get; set; } = new List<SuggestUserDto>();
    }
}
