using Domain.Contracts.Responses.User;

namespace Domain.Contracts.Responses.UserRelation
{
    public class MutualFriendReponse
    {
        public int MutualFriendCount { get; set; }
        public UserDto? User { get; set; }
    }
}
