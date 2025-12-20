namespace Domain.Enum.FriendRequest.Functions
{
    public enum RespondFriendRequestEnum
    {
        RespondFriendRequestSuccess,
        RespondFriendRequestFailed,
        FriendRequestNotFound,
        ReceiverNotFound,
        Unauthorized,
        InvalidStatus,
        AlreadyProcessed
    }

    public static class RespondFriendRequestEnumMessage
    {
        public static string GetMessage(this RespondFriendRequestEnum status)
        {
            return status switch
            {
                RespondFriendRequestEnum.RespondFriendRequestSuccess => "Friend request responded successfully.",
                RespondFriendRequestEnum.RespondFriendRequestFailed => "Failed to respond to friend request.",
                RespondFriendRequestEnum.FriendRequestNotFound => "Friend request not found.",
                RespondFriendRequestEnum.ReceiverNotFound => "Receiver not found.",
                RespondFriendRequestEnum.Unauthorized => "You are not authorized to respond to this request.",
                RespondFriendRequestEnum.InvalidStatus => "Invalid response status.",
                RespondFriendRequestEnum.AlreadyProcessed => "Friend request has already been processed.",
                _ => "Unknown error."
            };
        }
    }
}
