using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Message.Functions
{
    public enum SendMessageEnum
    {
        ReceiverNotFound,
        SenderNotFound,
        SendMessageFailed,
        SendMessageSuceeded,
    }
    public static class SendMessageEnumMessage
    {
        public static string GetMessage(this SendMessageEnum status)
        {
            return status switch
            {
                SendMessageEnum.ReceiverNotFound => "Receiver not found. Please check username again!",
                SendMessageEnum.SenderNotFound => "Sender not found. Please check username again!",
                SendMessageEnum.SendMessageFailed => "Send message failed. Please check your connection!",
                SendMessageEnum.SendMessageSuceeded => "Send message successfully!",
                _ => "An unknown error occurred"
            };
        }
    }
}
