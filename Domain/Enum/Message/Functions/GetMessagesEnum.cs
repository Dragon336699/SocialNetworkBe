using Domain.Enum.User.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Message.Functions
{
    public enum GetMessagesEnum
    {
        UserNotFound,
        GetMessageFail,
        GetMessageSuccess,
    }
    public static class GetMessagesMessage
    {
        public static string GetMessage(this GetMessagesEnum status)
        {
            return status switch
            {
                GetMessagesEnum.UserNotFound => "User Not Found.",
                GetMessagesEnum.GetMessageFail => "Get messages failed!",
                GetMessagesEnum.GetMessageSuccess => "Get messages successfully!",
                _ => "An unknown error occurred"
            };
        }
    }
}
