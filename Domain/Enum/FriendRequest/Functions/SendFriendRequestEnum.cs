using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.FriendRequest.Functions
{
    public enum SendFriendRequestEnum
    {
        SendFriendRequestSuccess,
        SendFriendRequestFailed,
        SenderNotFound,
        ReceiverNotFound,
        RequestAlreadyExists,
        AlreadyFriends,
        CannotSendToSelf,
        ReceiverBlocked
    }

    public static class SendFriendRequestEnumMessage
    {
        public static string GetMessage(this SendFriendRequestEnum status)
        {
            return status switch
            {
                SendFriendRequestEnum.SendFriendRequestSuccess => "Friend request sent successfully.",
                SendFriendRequestEnum.SendFriendRequestFailed => "Failed to send friend request.",
                SendFriendRequestEnum.SenderNotFound => "Sender not found.",
                SendFriendRequestEnum.ReceiverNotFound => "Receiver not found.",
                SendFriendRequestEnum.RequestAlreadyExists => "Friend request already exists.",
                SendFriendRequestEnum.AlreadyFriends => "You are already friends with this user.",
                SendFriendRequestEnum.CannotSendToSelf => "Cannot send friend request to yourself.",
                SendFriendRequestEnum.ReceiverBlocked => "Cannot send friend request to blocked user.",
                _ => "Unknown error."
            };
        }
    }
}
