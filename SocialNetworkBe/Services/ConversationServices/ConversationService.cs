using AutoMapper;
using Domain.Contracts.Responses.Conversation;
using Domain.Entities;
using Domain.Enum.Conversation.Functions;
using Domain.Enum.Conversation.Types;
using Domain.Interfaces.ServiceInterfaces;
using Domain.Interfaces.UnitOfWorkInterface;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace SocialNetworkBe.Services.ConversationServices
{
    public class ConversationService : IConversationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConversationUserService _conversationUserService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ILogger<ConversationService> _logger;

        public ConversationService(
            IUnitOfWork unitOfWork,
            IConversationUserService conversationUserService,
            IUserService userService,
            IMapper mapper,
            ILogger<ConversationService> logger)
        {
            _unitOfWork = unitOfWork;
            _conversationUserService = conversationUserService;
            _userService = userService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<(CreateConversationEnum, Guid?)> CreateConversationAsync(Guid senderId, string receiverUserName)
        {
            try
            {

                var (userFound, receiver) = await _userService.GetUserInfoByUserName(receiverUserName);
                if (!userFound || receiver == null)
                {
                    return (CreateConversationEnum.ReceiverNotFound, null);
                }

                Guid? existingConversationId = await _conversationUserService.CheckExist(senderId, receiver.Id);
                if (existingConversationId != null)
                {
                    return (CreateConversationEnum.ConversationExists, existingConversationId);
                }

                var conversation = new Conversation
                {
                    Id = Guid.NewGuid(),
                    Type = ConversationType.Personal,
                    CreatedAt = DateTime.Now,
                };
                _unitOfWork.ConversationRepository.Add(conversation);

                var userIds = new List<Guid> { senderId, receiver.Id };
                await _conversationUserService.AddUsersToConversationAsync(conversation.Id, userIds);

                return (CreateConversationEnum.CreateConversationSuccess, conversation.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating conversation");
                return (CreateConversationEnum.CreateConversationFailed, null);
            }
        }

        public async Task<Conversation?> GetConversationById(Guid conversationId)
        {
            try
            {
                Conversation? conversation = await _unitOfWork.ConversationRepository.GetByIdAsync(conversationId);
                return conversation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting conversation");
                throw;
            }
        }

        public async Task<List<ConversationDto>?> GetAllConversationByUser(Guid userId)
        {
            try
            {
                var conversationUser = await _unitOfWork.ConversationUserRepository.FindAsync(cu => cu.UserId == userId);
                if (conversationUser == null) return null;
                List<Conversation> conversations = new List<Conversation>();
                foreach (var item in conversationUser)
                {
                    Conversation? conversation = null;
                    Guid conversationId = item.ConversationId;
                    conversation = await _unitOfWork.ConversationRepository.GetByIdAsync(conversationId);
                    if (conversation.Type == ConversationType.Personal)
                    {
                        IEnumerable<Conversation>? conversationsIenum = await _unitOfWork.ConversationRepository.FindAsyncWithIncludes(c => c.Id == conversationId, c => c.ConversationUsers);
                        conversation = conversationsIenum?.FirstOrDefault();
                    }
                    if (conversation != null) conversations.Add(conversation);
                }

                return _mapper.Map<List<ConversationDto>>(conversations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all conversations");
                throw;
            }
        }
    }
}