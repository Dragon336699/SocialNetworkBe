using Azure.Core;
using Domain.Contracts.Requests.ConversationUser;
using Domain.Contracts.Requests.Message;
using Domain.Contracts.Responses.Message;
using Domain.Contracts.Responses.User;
using Domain.Entities;
using Domain.Enum.Conversation.Types;
using Domain.Enum.Message.Functions;
using Domain.Enum.Message.Types;
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
        private readonly IConversationService _conversationService;
        private readonly IUserService _userService;
        private readonly ILogger<ChatHub> _logger;
        public ChatHub(IMessageService messageService, IConversationUserService conversationUserService, IConversationService conversationService, IUserService userService, ILogger<ChatHub> logger)
        {
            _messageService = messageService;
            _conversationUserService = conversationUserService;
            _conversationService = conversationService;
            _userService = userService;
            _logger = logger;
        }

        public async Task<bool?> UpdateMessageStatus(UpdateMessageStatusRequest request)
        {
            bool? updatedMessageStatus = await _messageService.UpdateMessage(request.MessageId, request.Status);
            if (updatedMessageStatus == null) return false;

            MessageDto? updatedMessage = await _messageService.GetMessageById(request.MessageId);
            if (updatedMessage == null) return false;
            await Clients.User(updatedMessage.SenderId.ToString().ToLower()).SendAsync("UpdatedMessage", updatedMessage);
            return updatedMessageStatus;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            await Clients.All.SendAsync("ReceiveMessage", $"{userId} has joined");
        }
    }
}
