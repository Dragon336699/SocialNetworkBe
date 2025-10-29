using Azure.Core;
using Domain.Contracts.Requests.ConversationUser;
using Domain.Contracts.Requests.Message;
using Domain.Contracts.Responses.Message;
using Domain.Contracts.Responses.User;
using Domain.Entities;
using Domain.Enum.Conversation.Types;
using Domain.Enum.Message.Functions;
using Domain.Enum.Message.Types;
using Domain.Interfaces.ChatInterfaces;
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
        private readonly IUserConnectionManager _userConnectionManager;
        private readonly IUserService _userService;
        private readonly ILogger<ChatHub> _logger;
        public ChatHub(IMessageService messageService, IConversationUserService conversationUserService, IConversationService conversationService, IUserConnectionManager userConnectionManager, IUserService userService, ILogger<ChatHub> logger)
        {
            _messageService = messageService;
            _conversationUserService = conversationUserService;
            _conversationService = conversationService;
            _userConnectionManager = userConnectionManager;
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
            try
            {
                var userIdentifier = Context.UserIdentifier;
                if (!Guid.TryParse(Context.UserIdentifier, out Guid userId))
                    return;
                var connectionIdContext = Context.ConnectionId;
                await _userConnectionManager.AddConnectionAsync(userIdentifier, connectionIdContext);
                IEnumerable<string> connectionIds = await _userConnectionManager.GetConnectionAsync(userId);
                IEnumerable<ConversationUser>? conversationUsers = await _conversationUserService.GetConversationUsersByUserId(userId);
                var tasks = new List<Task>();
                foreach (var item in conversationUsers)
                {
                    foreach (var connectionId in connectionIds)
                    {
                        tasks.Add(Groups.AddToGroupAsync(connectionId, item.ConversationId.ToString()));
                    }
                }
                // Chạy song song thay vì đợi add từng cái
                await Task.WhenAll(tasks);
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured when connect to server!");
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                var userIdentifier = Context.UserIdentifier;
                if (!Guid.TryParse(Context.UserIdentifier, out Guid userId))
                    return;
                var connectionIdContext = Context.ConnectionId;
                await _userConnectionManager.RemoveConnectionAsync(userIdentifier, connectionIdContext);
                await base.OnDisconnectedAsync(exception);
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured when disconnect to server!");
            }
        }
    }
}
