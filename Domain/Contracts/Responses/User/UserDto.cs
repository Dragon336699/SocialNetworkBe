namespace Domain.Contracts.Responses.User
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public required string Email { get; set; }
        public required string UserName { get; set; }
        public required string Status { get; set; }
        public required string FirstName { get; set; }
        public string? LastName { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Gender { get; set; }
        public string? Description { get; set; }
        public string? PhoneNumber { get; set; }

    }
}
