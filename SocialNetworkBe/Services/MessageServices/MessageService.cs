using AutoMapper;
using Domain.Contracts.Requests.Message;
using Domain.Contracts.Responses.Message;
using Domain.Entities;
using Domain.Enum.Message.Functions;
using Domain.Enum.Message.Types;
using Domain.Interfaces.ServiceInterfaces;
using Domain.Interfaces.UnitOfWorkInterface;

namespace SocialNetworkBe.Services.MessageService
{
    public class MessageService : IMessageService
    {
        private readonly IUserService _userService;
        private readonly ILogger<MessageService> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public MessageService(IUserService userService, ILogger<MessageService> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userService = userService;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<(GetMessagesEnum, List<MessageDto>?)> GetMessages(GetMessagesRequest request)
        {
            try
            {
                var (status, receiver) = await _userService.GetUserInfoByUserName(request.ReceiverUserName);
                if (!status || receiver == null) return (GetMessagesEnum.UserNotFound, null);
                List<Message>? messages = await _unitOfWork.MessageRepository.GetMessages(request.UserId, receiver.Id, request.Skip, request.Take);
                List<MessageDto>? messagesDto = _mapper.Map<List<MessageDto>>(messages);
                return (GetMessagesEnum.GetMessageSuccess, messagesDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while getting messages");
                throw;
            }
        }

        public MessageDto? SaveMessage(SendMessageRequest request, Guid conversationId, Guid receiverId) 
        {
            try
            {
                Message message = new Message
                {
                    Content = request.Content,
                    Status = MessageStatus.Sent,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    ConversationId = conversationId,
                    SenderId = request.SenderId,
                    ReceiverId = receiverId
                };

                _unitOfWork.MessageRepository.Add(message);
                int rowEffected = _unitOfWork.Complete();
                if (rowEffected > 0)
                {
                    var messageDto = _mapper.Map<MessageDto>(message);
                    return messageDto;
                };
                return null;
            } catch (Exception ex)
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
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while updating messages");
                throw;
            }
        }

        public async Task<MessageDto?> GetMessageById(Guid messageId)
        {
            try
            {
                Message? message = await _unitOfWork.MessageRepository.GetByIdAsync(messageId);
                if (message == null) return null;
                var messageDto = _mapper.Map<MessageDto>(message);
                return messageDto;
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while getting messages");
                throw;
            }
        }
    }
}
