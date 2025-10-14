using Domain.Entities;
using Domain.Enum.Conversation;
using Domain.Enum.Conversation.Functions;
using Domain.Enum.Conversation.Types;
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
            _unitOfWork = unitOfWork;
            _conversationRepository = _unitOfWork.Conversations;
            _logger = logger;
        }

        public async Task<ConversationStatus> CreateConversationAsync(Guid user1Id, Guid user2Id, ConversationType type)
        {
            try
            {
                // Kiểm tra nếu hai ID người dùng giống nhau, trả về lỗi đầu vào không hợp lệ
                if (user1Id == user2Id)
                {
                    return ConversationStatus.InvalidInput;
                }

                // Kiểm tra xem cuộc hội thoại đã tồn tại giữa hai người dùng chưa
                var existingConversation = await _conversationRepository.GetConversationBetweenUsersAsync(user1Id, user2Id);
                if (existingConversation != null)
                {
                    return ConversationStatus.AlreadyExists;
                }

                // Tạo mới một đối tượng Conversation với loại được chỉ định
                var conversation = new Conversation
                {
                    Type = type,  
                    CreatedAt = DateTime.UtcNow 
                };

                // Khởi tạo danh sách ConversationUsers với vai trò linh hoạt
                conversation.ConversationUsers = new List<ConversationUser>
                {
                    new ConversationUser { UserId = user1Id, JoinedAt = DateTime.UtcNow, RoleName = ConversationRole.Administrator },
                    new ConversationUser { UserId = user2Id, JoinedAt = DateTime.UtcNow, RoleName = ConversationRole.User }
                };

                // Nếu là Group
                if (type == ConversationType.Group)
                {
                 
                }
            
                _conversationRepository.Add(conversation);

                // Lưu thay đổi vào cơ sở dữ liệu (dùng Complete thay vì SaveChangesAsync)
                int affectedRows = _unitOfWork.Complete();
                if (affectedRows == 0)
                {                
                    return ConversationStatus.InvalidInput;
                }            
                return ConversationStatus.CreatedSuccessfully;
            }
            catch (Exception ex)
            {               
                _logger.LogError(ex, "Error creating conversation between users {User1Id} and {User2Id}", user1Id, user2Id);
                throw;
            }
        }
    }
}