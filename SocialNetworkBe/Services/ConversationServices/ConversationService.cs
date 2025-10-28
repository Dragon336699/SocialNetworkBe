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

        public async Task<(CreateConversationEnum, Guid?)> CreateConversationAsync(ConversationType conversationType, List<Guid> userIds)
        {
            try
            {
                foreach (var userId in userIds)
                {
                    var userInfo = await _userService.GetUserInfoByUserId(userId.ToString());
                    if (userInfo == null)
                    {
                        return (CreateConversationEnum.ReceiverNotFound, null);
                    }
                }

                if (conversationType == ConversationType.Personal && userIds.Count == 2)
                {
                    Guid? existingConversationId = await _conversationUserService.CheckExist(userIds[0], userIds[1]);
                    if (existingConversationId != null)
                    {
                        return (CreateConversationEnum.ConversationExists, existingConversationId);
                    }
                }

                var conversation = new Conversation
                {
                    Id = Guid.NewGuid(),
                    Type = conversationType,
                    CreatedAt = DateTime.Now,
                };
                _unitOfWork.ConversationRepository.Add(conversation);

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
                List<ConversationDto>? conversations = await _unitOfWork.ConversationRepository.GetAllConversationByUser(userId);
                //foreach (var item in conversationUser)
                //{
                //    Conversation? conversation = null;
                //    Guid conversationId = item.ConversationId;
                //    conversation = await _unitOfWork.ConversationRepository.GetByIdAsync(conversationId);
                //    if (conversation.Type == ConversationType.Personal)
                //    {
                //        conversation = conversationsIenum?.FirstOrDefault();
                //    }
                //    if (conversation != null) conversations.Add(conversation);
                //}

                return conversations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all conversations");
                throw;
            }
        }
    }
}