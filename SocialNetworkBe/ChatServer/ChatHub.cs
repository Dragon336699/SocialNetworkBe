using Azure.Core;
using Domain.Contracts.Requests.Message;
using Domain.Contracts.Responses.Message;
using Domain.Contracts.Responses.User;
using Domain.Entities;
using Domain.Enum.Message.Functions;
using Domain.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SocialNetworkBe.ChatServer
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;
        private readonly IConversationUserService _conversationUserService;
        private readonly IUserService _userService;
        private readonly ILogger<ChatHub> _logger;
        public ChatHub(IMessageService messageService, IConversationUserService conversationUserService, IUserService userService, ILogger<ChatHub> logger)
        {
            _messageService = messageService;
            _conversationUserService = conversationUserService;
            _userService = userService;
            _logger = logger;
        }
        public async Task<SendMessageResponse> SendMessage(SendMessageRequest request)
        {
            try
            {
                var receiverInfo = await _userService.GetUserInfoByUserId(request.ReceiverId.ToString());
                var senderInfo = await _userService.GetUserInfoByUserId(request.SenderId.ToString());
                if (receiverInfo == null) return new SendMessageResponse { Status = false, Message = SendMessageEnum.ReceiverNotFound.GetMessage(), NewMessage = null};
                if (senderInfo == null) return new SendMessageResponse { Status = false, Message = SendMessageEnum.SenderNotFound.GetMessage(), NewMessage = null };
                Guid? conversationId = await _conversationUserService.CheckExist(request.SenderId, request.ReceiverId);
                if (conversationId == null)
                {
                    //conversationId = Guid.NewGuid();
                    //Logic tạo conversationUser và conversation
                };
                
                await Clients.User(receiverInfo.Id.ToString().ToLower()).SendAsync("ReceivePrivateMessage", request.Content);

                MessageDto? newMessage = _messageService.SaveMessage(request, conversationId.Value, request.ReceiverId);
                return new SendMessageResponse { Status = true, Message = SendMessageEnum.SendMessageSuceeded.GetMessage(), NewMessage = newMessage };
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error while sending message at chathub");
                return new SendMessageResponse { Status = false, Message = SendMessageEnum.SendMessageFailed.GetMessage(), NewMessage = null };
            }
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            await Clients.All.SendAsync("ReceiveMessage", $"{userId} has joined");
        }
    }
}
