using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Notification.Types
{
    public enum NotificationType
    {
        LikePost,
        CommentPost,
        ReplyComment,
        AddFriendRequest,
        AcceptFriendRequest,
        GroupInvite
    }
}
