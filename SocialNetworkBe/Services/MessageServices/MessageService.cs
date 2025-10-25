using AutoMapper;
using Domain.Contracts.Requests.ConversationUser;
using Domain.Contracts.Requests.Message;
using Domain.Contracts.Responses.Message;
using Domain.Entities;
using Domain.Enum.Conversation.Types;
using Domain.Enum.Message.Functions;
using Domain.Enum.Message.Types;
using Domain.Enum.MessageAttachment.Types;
using Domain.Interfaces.ServiceInterfaces;
using Domain.Interfaces.UnitOfWorkInterface;
using Microsoft.AspNetCore.SignalR;
using SocialNetworkBe.ChatServer;

namespace SocialNetworkBe.Services.MessageService
{
    public class MessageService : IMessageService
    {
        private readonly IUserService _userService;
        private readonly IConversationService _conversationService;
        private readonly IConversationUserService _conversationUserService;
        private readonly IUploadService _uploadService;
        private readonly ILogger<MessageService> _logger;
        private readonly IHubContext<ChatHub> _chatHub;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public MessageService(
            IUserService userService,
            ILogger<MessageService> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IConversationService conversationService,
            IConversationUserService conversationUserService,
            IUploadService uploadService,
            IHubContext<ChatHub> chatHub)
        {
            _userService = userService;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _conversationService = conversationService;
            _conversationUserService = conversationUserService;
            _uploadService = uploadService;
            _chatHub = chatHub;
        }

        public async Task<(GetMessagesEnum, List<MessageDto>?)> GetMessages(GetMessagesRequest request)
        {
            try
            {
                List<Message>? messages = await _unitOfWork.MessageRepository.GetMessages(request.ConversationId, request.Skip, request.Take);
                List<MessageDto>? messagesDto = _mapper.Map<List<MessageDto>>(messages);
                return (GetMessagesEnum.GetMessageSuccess, messagesDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while getting messages");
                throw;
            }
        }

        public MessageDto? SaveMessage(SendMessageRequest request)
        {
            try
            {
                Message message = new Message
                {
                    Content = request.Content == null ? "" : request.Content,
                    Status = MessageStatus.Sent,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    RepliedMessageId = request.RepliedMessageId,
                    ConversationId = request.ConversationId,
                    SenderId = request.SenderId,
                };

                _unitOfWork.MessageRepository.Add(message);
                int rowEffected = _unitOfWork.Complete();
                if (rowEffected > 0)
                {
                    var messageDto = _mapper.Map<MessageDto>(message);
                    return messageDto;
                };
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while saving messages");
                throw;
            }
        }

        public async Task<bool> UpdateMessage(Guid messageId, MessageStatus status)
        {
            try
            {
                Message? updatedMessage = await _unitOfWork.MessageRepository.UpdateAllMessagesStatus(messageId, status);
                if (updatedMessage == null) return false;
                _unitOfWork.Complete();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while updating messages");
                throw;
            }
        }

        public async Task<MessageDto?> GetMessageById(Guid messageId)
        {
            try
            {
                IEnumerable<Message>? messages = await _unitOfWork.MessageRepository.FindAsyncWithIncludes(m => m.Id == messageId, m => m.Sender, m => m.MessageAttachments);

                Message? message = messages?.FirstOrDefault();
                if (message == null) return null;
                var messageDto = _mapper.Map<MessageDto>(message);
                return messageDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while getting messages");
                throw;
            }
        }

        public async Task<(SendMessageEnum, MessageDto?)> SendMessage(SendMessageRequest request)
        {
            try
            {
                List<string>? fileUrls = null;
                if (request.Files != null)
                {

                    var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var audioExtensions = new[] { ".wav", ".mp3", ".ogg", ".m4a" };
                    foreach (var file in request.Files)
                    {
                        var extension = Path.GetExtension(file.FileName).ToLower();
                        if (imageExtensions.Contains(extension))
                        {
                            fileUrls = await _uploadService.UploadFile(request.Files, "messages/images");
                        }
                        else if (audioExtensions.Contains(extension))
                        {
                            fileUrls = await _uploadService.UploadFile(request.Files, "messages/voices");
                        }
                    }


                }
                var senderInfo = await _userService.GetUserInfoByUserId(request.SenderId.ToString());
                if (senderInfo == null) return (SendMessageEnum.SenderNotFound, null);
                Conversation? conversation = await _conversationService.GetConversationById(request.ConversationId);
                IEnumerable<ConversationUser>? conversationUser = await _conversationUserService.GetConversationUser(new GetConversationUserRequest { SenderId = request.SenderId, ConversationId = request.ConversationId, ConversationType = conversation.Type });
                if (conversation == null)
                {
                    //conversationId = Guid.NewGuid();
                    //Logic tạo conversationUser và conversation
                };

                MessageDto? newMessage = SaveMessage(request);
                if (newMessage == null) return (SendMessageEnum.SendMessageFailed, null);

                if (fileUrls != null)
                {
                    foreach (var imageUrl in fileUrls)
                    {
                        MessageAttachment messageAttachment = new MessageAttachment
                        {
                            MessageId = newMessage.Id,
                            FileUrl = imageUrl,
                            FileType = request.FileType ?? FileTypes.File
                        };
                        _unitOfWork.MessageAttachmentRepository.Add(messageAttachment);
                        newMessage?.MessageAttachments?.Add(messageAttachment);
                    }
                }
                _unitOfWork.Complete();
                if (conversation?.Type == ConversationType.Personal)
                {
                    var conversationReceiver = conversationUser.FirstOrDefault(cu => cu.UserId != request.SenderId);
                    await _chatHub.Clients.User(conversationReceiver.UserId.ToString().ToLower()).SendAsync("ReceivePrivateMessage", newMessage);
                }
                else
                {
                    await _chatHub.Clients.Group(conversation.Id.ToString().ToLower()).SendAsync("ReceivePrivateMessage", newMessage);
                }

                return (SendMessageEnum.SendMessageSuceeded, newMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while sending message");
                return (SendMessageEnum.SendMessageFailed, null);
            }
        }
    }
}
