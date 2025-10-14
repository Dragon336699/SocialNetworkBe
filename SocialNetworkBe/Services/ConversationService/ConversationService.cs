using Domain.Entities;
using Domain.Enum.Conversation.Functions;
using Domain.Enum.Conversation.Types;
using Domain.Enum.Message.Types;
using Domain.Interfaces.RepositoryInterfaces;
using Domain.Interfaces.UnitOfWorkInterface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialNetworkBe.Services
{
    public class ConversationService
    {
        private readonly IUnitOfWork _unitOfWork;  
        private readonly IConversationRepository _conversationRepository;  
        private readonly ILogger<ConversationService> _logger; 

        public ConversationService(IUnitOfWork unitOfWork, ILogger<ConversationService> logger)
        {
            // Khởi tạo UnitOfWork và Logger thông qua Dependency Injection
            _unitOfWork = unitOfWork;
            _conversationRepository = _unitOfWork.Conversations ;
            _logger = logger;
        }

        public async Task<ConversationStatus> CreateConversationAsync(Guid senderId, Guid receiverId, string content, ConversationType type)
        {
            try
            {
                // Kiểm tra nếu hai ID người dùng giống nhau, trả về lỗi đầu vào không hợp lệ
                if (senderId == receiverId)
                {
                    return ConversationStatus.InvalidInput;
                }

                // Kiểm tra xem cuộc hội thoại đã tồn tại chưa
                var existingConversation = await _conversationRepository.GetConversationBetweenUsersAsync(senderId, receiverId);
                Conversation conversation;

                if (existingConversation == null)
                {
                    // Tạo mới cuộc hội thoại nếu chưa tồn tại
                    conversation = new Conversation
                    {
                        Type = type,
                        CreatedAt = DateTime.UtcNow
                    };
                    conversation.ConversationUsers = new List<ConversationUser>
                    {
                        new ConversationUser { UserId = senderId, JoinedAt = DateTime.UtcNow, RoleName = ConversationRole.Administrator },
                        new ConversationUser { UserId = receiverId, JoinedAt = DateTime.UtcNow, RoleName = ConversationRole.User }
                    };
                    _conversationRepository.Add(conversation);
                }
                else
                {
                    conversation = existingConversation;
                }

                // Tạo và lưu tin nhắn
                var message = new Message
                {
                    ConversationId = conversation.Id,
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    Content = content,
                    Status = MessageStatus.Sent.ToString(), 
                    CreatedAt = DateTime.UtcNow
                };
                await _conversationRepository.AddOrUpdateMessageAsync(message);

                // Lưu thay đổi vào cơ sở dữ liệu
                int affectedRows = _unitOfWork.Complete();
                if (affectedRows == 0)
                {
                    return ConversationStatus.InvalidInput;
                }

                return ConversationStatus.CreatedSuccessfully;
            }
            catch (Exception ex)
            {               
                _logger.LogError(ex, "Error creating conversation between users {SenderId} and {ReceiverId}", senderId, receiverId);
                throw;
            }
        }
    }
}