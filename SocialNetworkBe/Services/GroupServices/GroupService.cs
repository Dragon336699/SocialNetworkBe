using AutoMapper;
using Domain.Contracts.Requests.Group;
using Domain.Contracts.Responses.Group;
using Domain.Entities;
using Domain.Enum.Group.Functions;
using Domain.Enum.Group.Types;
using Domain.Enum.Notification.Types;
using Domain.Interfaces.BuilderInterfaces;
using Domain.Interfaces.ServiceInterfaces;
using Domain.Interfaces.UnitOfWorkInterface;

namespace SocialNetworkBe.Services.GroupServices
{
    public class GroupService : IGroupService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GroupService> _logger;
        private readonly IMapper _mapper;
        private readonly IUploadService _uploadService;
        private readonly IServiceProvider _serviceProvider;

        public GroupService(
            IUnitOfWork unitOfWork,
            ILogger<GroupService> logger,
            IMapper mapper,
            IUploadService uploadService,
            IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _uploadService = uploadService;
            _serviceProvider = serviceProvider;
        }

        public async Task<(CreateGroupEnum, Guid?)> CreateGroupAsync(CreateGroupRequest request, Guid userId)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return (CreateGroupEnum.UserNotFound, null);
                }

                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return (CreateGroupEnum.InvalidName, null);
                }

                if (string.IsNullOrWhiteSpace(request.Description))
                {
                    return (CreateGroupEnum.InvalidDescription, null);
                }

                string imageUrl = "default-group-image.jpg";

                if (request.Image != null)
                {
                    var validImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
                    var fileExtension = Path.GetExtension(request.Image.FileName).ToLower();

                    if (!validImageExtensions.Contains(fileExtension))
                    {
                        return (CreateGroupEnum.InvalidImageFormat, null);
                    }

                    const long maxFileSize = 10 * 1024 * 1024;
                    if (request.Image.Length > maxFileSize)
                    {
                        return (CreateGroupEnum.FileTooLarge, null);
                    }

                    var uploadResult = await _uploadService.UploadFile(
                        new List<IFormFile> { request.Image },
                        "groups/images"
                    );

                    if (uploadResult == null || !uploadResult.Any())
                    {
                        return (CreateGroupEnum.ImageUploadFailed, null);
                    }

                    imageUrl = uploadResult.First();
                }

                var group = _mapper.Map<Group>(request);
                group.ImageUrl = imageUrl;

                _unitOfWork.GroupRepository.Add(group);

                var saveGroupResult = await _unitOfWork.CompleteAsync();
                if (saveGroupResult <= 0)
                {
                    return (CreateGroupEnum.CreateGroupFailed, null);
                }

                var groupUser = new GroupUser
                {
                    UserId = userId,
                    GroupId = group.Id,
                    RoleName = GroupRole.SuperAdministrator,
                    JoinedAt = DateTime.UtcNow
                };

                _unitOfWork.GroupUserRepository.Add(groupUser);

                var result = await _unitOfWork.CompleteAsync();
                if (result > 0)
                {
                    return (CreateGroupEnum.CreateGroupSuccess, group.Id);
                }

                return (CreateGroupEnum.CreateGroupFailed, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when creating group for user {UserId}", userId);
                return (CreateGroupEnum.CreateGroupFailed, null);
            }
        }

        public async Task<(GetAllGroupsEnum, List<GroupDto>?)> GetAllGroupsAsync(int skip = 0, int take = 10)
        {
            try
            {
                var groups = await _unitOfWork.GroupRepository.GetGroupsWithFullDetailsAsync(
                    g => g.IsPublic == 1,
                    skip,
                    take
                );

                if (groups == null || !groups.Any())
                {
                    return (GetAllGroupsEnum.NoGroupsFound, null);
                }

                var groupDtos = _mapper.Map<List<GroupDto>>(groups);

                return (GetAllGroupsEnum.Success, groupDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting all groups");
                return (GetAllGroupsEnum.Failed, null);
            }
        }

        public async Task<(GetGroupByIdEnum, GroupDto?)> GetGroupByIdAsync(Guid groupId, Guid userId)
        {
            try
            {
                var group = await _unitOfWork.GroupRepository.GetGroupWithFullDetailsByIdAsync(groupId);

                if (group == null)
                {
                    return (GetGroupByIdEnum.GroupNotFound, null);
                }

                if (group.IsPublic == 0)
                {
                    var isMember = group.GroupUsers?.Any(gu => gu.UserId == userId && gu.RoleName != GroupRole.Pending && gu.RoleName != GroupRole.Inviting) ?? false;
                    if (!isMember)
                    {
                        return (GetGroupByIdEnum.Unauthorized, null);
                    }
                }

                var groupDto = _mapper.Map<GroupDto>(group);

                return (GetGroupByIdEnum.Success, groupDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting group {GroupId}", groupId);
                return (GetGroupByIdEnum.Failed, null);
            }
        }

        public async Task<(UpdateGroupEnum, GroupDto?)> UpdateGroupAsync(Guid groupId, UpdateGroupRequest request, Guid userId)
        {
            try
            {                
                var groups = await _unitOfWork.GroupRepository.FindAsyncWithIncludes(
                    g => g.Id == groupId,
                    g => g.GroupUsers,
                    g => g.Posts
                );

                var group = groups?.FirstOrDefault();
                if (group == null)
                {
                    return (UpdateGroupEnum.GroupNotFound, null);
                }

                var groupUser = group.GroupUsers?.FirstOrDefault(gu => gu.UserId == userId);
                if (groupUser == null ||
                    (groupUser.RoleName != GroupRole.Administrator &&
                     groupUser.RoleName != GroupRole.SuperAdministrator))
                {
                    return (UpdateGroupEnum.Unauthorized, null);
                }

                if (!string.IsNullOrWhiteSpace(request.Name) && request.Name.Trim().Length == 0)
                {
                    return (UpdateGroupEnum.InvalidName, null);
                }

                if (!string.IsNullOrWhiteSpace(request.Description) && request.Description.Trim().Length == 0)
                {
                    return (UpdateGroupEnum.InvalidDescription, null);
                }

                if (request.RemoveImage)
                {
                    group.ImageUrl = "default-group-image.jpg";
                }
                else if (request.NewImage != null)
                {
                    var validImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
                    var fileExtension = Path.GetExtension(request.NewImage.FileName).ToLower();

                    if (!validImageExtensions.Contains(fileExtension))
                    {
                        return (UpdateGroupEnum.InvalidImageFormat, null);
                    }

                    const long maxFileSize = 10 * 1024 * 1024;
                    if (request.NewImage.Length > maxFileSize)
                    {
                        return (UpdateGroupEnum.FileTooLarge, null);
                    }

                    var uploadResult = await _uploadService.UploadFile(
                        new List<IFormFile> { request.NewImage },
                        "groups/images"
                    );

                    if (uploadResult == null || !uploadResult.Any())
                    {
                        return (UpdateGroupEnum.ImageUploadFailed, null);
                    }

                    group.ImageUrl = uploadResult.First();
                }

                _mapper.Map(request, group);

                _unitOfWork.GroupRepository.Update(group);
                var result = await _unitOfWork.CompleteAsync();

                if (result > 0)
                {
                    var groupDto = _mapper.Map<GroupDto>(group);
                    return (UpdateGroupEnum.UpdateGroupSuccess, groupDto);
                }

                return (UpdateGroupEnum.UpdateGroupFailed, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when updating group {GroupId}", groupId);
                return (UpdateGroupEnum.UpdateGroupFailed, null);
            }
        }

        public async Task<(DeleteGroupEnum, bool)> DeleteGroupAsync(Guid groupId, Guid userId)
        {
            try
            {
                var groups = await _unitOfWork.GroupRepository.FindAsyncWithIncludes(
                    g => g.Id == groupId,
                    g => g.GroupUsers
                );

                var group = groups?.FirstOrDefault();
                if (group == null)
                {
                    return (DeleteGroupEnum.GroupNotFound, false);
                }

                var groupUser = group.GroupUsers?.FirstOrDefault(gu => gu.UserId == userId);
                if (groupUser == null || groupUser.RoleName != GroupRole.SuperAdministrator)
                {
                    return (DeleteGroupEnum.Unauthorized, false);
                }

                _unitOfWork.GroupRepository.Remove(group);
                var result = await _unitOfWork.CompleteAsync();

                if (result > 0)
                {
                    return (DeleteGroupEnum.DeleteGroupSuccess, true);
                }

                return (DeleteGroupEnum.DeleteGroupFailed, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when deleting group {GroupId}", groupId);
                return (DeleteGroupEnum.DeleteGroupFailed, false);
            }
        }

        public async Task<(JoinGroupEnum, bool)> JoinGroupAsync(Guid groupId, Guid userId)
        {
            try
            {
                var groups = await _unitOfWork.GroupRepository.FindAsyncWithIncludes(
                    g => g.Id == groupId,
                    g => g.GroupUsers
                );
                
                var group = groups?.FirstOrDefault();
                if (group == null)
                {
                    return (JoinGroupEnum.GroupNotFound, false);
                }

                var existingMember = group.GroupUsers?.FirstOrDefault(gu => gu.UserId == userId);

                if (existingMember != null)
                {
                    if (existingMember.RoleName == GroupRole.Pending)
                    {
                        return (JoinGroupEnum.AlreadyRequested, false);
                    }
                    return (JoinGroupEnum.AlreadyMember, false);
                }

                var groupUser = new GroupUser
                {
                    UserId = userId,
                    GroupId = groupId,
                    RoleName = GroupRole.Pending,
                    JoinedAt = DateTime.UtcNow
                };

                _unitOfWork.GroupUserRepository.Add(groupUser);
                var result = await _unitOfWork.CompleteAsync();

                if (result > 0)
                {                  
                    try
                    {
                        var requester = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                        if (requester != null)
                        {                           
                            var adminIds = group.GroupUsers?
                                .Where(gu => gu.RoleName == GroupRole.Administrator || gu.RoleName == GroupRole.SuperAdministrator)
                                .Select(gu => gu.UserId)
                                .ToList() ?? new List<Guid>();

                            if (adminIds.Any())
                            {
                                using (var scope = _serviceProvider.CreateScope())
                                {
                                    var notificationDataBuilder = scope.ServiceProvider.GetRequiredService<INotificationDataBuilder>();
                                    var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                                    var notificationData = notificationDataBuilder.BuilderDataForGroupJoinRequest(group, requester);

                                    await notificationService.ProcessAndSendNotiForGroupJoinRequest(
                                        NotificationType.GroupJoinRequest,
                                        notificationData,
                                        $"/groups/{groupId}",
                                        adminIds,
                                        groupId
                                    );
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error sending group join request notification");
                    }

                    return (JoinGroupEnum.JoinRequestSent, true);
                }

                return (JoinGroupEnum.JoinGroupFailed, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when user {UserId} joining group {GroupId}", userId, groupId);
                return (JoinGroupEnum.JoinGroupFailed, false);
            }
        }

        public async Task<(ApproveJoinRequestEnum, GroupUserDto?)> ApproveJoinRequestAsync(
            Guid groupId,
            Guid targetUserId,
            Guid currentUserId)
        {
            try
            {
                var groups = await _unitOfWork.GroupRepository.FindAsyncWithIncludes(
                    g => g.Id == groupId,
                    g => g.GroupUsers
                );

                var group = groups?.FirstOrDefault();
                if (group == null)
                {
                    return (ApproveJoinRequestEnum.GroupNotFound, null);
                }
              
                var currentUserRole = group.GroupUsers?.FirstOrDefault(gu => gu.UserId == currentUserId);
                if (currentUserRole == null ||
                    (currentUserRole.RoleName != GroupRole.SuperAdministrator &&
                     currentUserRole.RoleName != GroupRole.Administrator))
                {
                    return (ApproveJoinRequestEnum.Unauthorized, null);
                }
               
                var joinRequest = group.GroupUsers?.FirstOrDefault(
                    gu => gu.UserId == targetUserId && gu.RoleName == GroupRole.Pending);

                if (joinRequest == null)
                {                
                    var existingMember = group.GroupUsers?.FirstOrDefault(gu => gu.UserId == targetUserId);
                    if (existingMember != null && existingMember.RoleName != GroupRole.Pending)
                    {
                        return (ApproveJoinRequestEnum.AlreadyMember, null);
                    }
                    return (ApproveJoinRequestEnum.RequestNotFound, null);
                }
               
                joinRequest.RoleName = GroupRole.User;
                joinRequest.JoinedAt = DateTime.UtcNow;

                _unitOfWork.GroupUserRepository.Update(joinRequest);
                var result = await _unitOfWork.CompleteAsync();

                if (result > 0)
                {              
                    try
                    {
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var notificationDataBuilder = scope.ServiceProvider.GetRequiredService<INotificationDataBuilder>();
                            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                            var notificationData = notificationDataBuilder.BuilderDataForGroupJoinRequestAccepted(group);

                            await notificationService.ProcessAndSendNotiForGroupJoinRequestAccepted(
                                NotificationType.GroupJoinRequestAccepted,
                                notificationData,
                                $"/groups/{groupId}",
                                targetUserId,
                                groupId
                            );
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error sending group join request accepted notification");
                    }

                    var updatedGroupUser = await _unitOfWork.GroupUserRepository.FindFirstAsyncWithIncludes(
                        gu => gu.GroupId == groupId && gu.UserId == targetUserId,
                        gu => gu.User
                    );

                    var groupUserDto = _mapper.Map<GroupUserDto>(updatedGroupUser);
                    return (ApproveJoinRequestEnum.Success, groupUserDto);
                }

                return (ApproveJoinRequestEnum.Failed, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when approving join request for user {TargetUserId} in group {GroupId}",
                    targetUserId, groupId);
                return (ApproveJoinRequestEnum.Failed, null);
            }
        }
        public async Task<(RejectJoinRequestEnum, bool)> RejectJoinRequestAsync(
            Guid groupId,
            Guid targetUserId,
            Guid currentUserId)
        {
            try
            {
                var groups = await _unitOfWork.GroupRepository.FindAsyncWithIncludes(
                    g => g.Id == groupId,
                    g => g.GroupUsers
                );

                var group = groups?.FirstOrDefault();
                if (group == null)
                {
                    return (RejectJoinRequestEnum.GroupNotFound, false);
                }
             
                var currentUserRole = group.GroupUsers?.FirstOrDefault(gu => gu.UserId == currentUserId);
                if (currentUserRole == null ||
                    (currentUserRole.RoleName != GroupRole.SuperAdministrator &&
                     currentUserRole.RoleName != GroupRole.Administrator))
                {
                    return (RejectJoinRequestEnum.Unauthorized, false);
                }
              
                var joinRequest = group.GroupUsers?.FirstOrDefault(
                    gu => gu.UserId == targetUserId && gu.RoleName == GroupRole.Pending);

                if (joinRequest == null)
                {
                    return (RejectJoinRequestEnum.RequestNotFound, false);
                }
               
                _unitOfWork.GroupUserRepository.Remove(joinRequest);
                var result = await _unitOfWork.CompleteAsync();

                if (result > 0)
                {
                    return (RejectJoinRequestEnum.Success, true);
                }

                return (RejectJoinRequestEnum.Failed, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when rejecting join request for user {TargetUserId} in group {GroupId}",
                    targetUserId, groupId);
                return (RejectJoinRequestEnum.Failed, false);
            }
        }
     
        public async Task<(CancelJoinRequestEnum, bool)> CancelJoinRequestAsync(Guid groupId, Guid userId)
        {
            try
            {
                var group = await _unitOfWork.GroupRepository.GetByIdAsync(groupId);
                if (group == null)
                {
                    return (CancelJoinRequestEnum.GroupNotFound, false);
                }

                var joinRequest = await _unitOfWork.GroupUserRepository
                    .FindFirstAsync(gu => gu.GroupId == groupId &&
                                         gu.UserId == userId &&
                                         gu.RoleName == GroupRole.Pending);

                if (joinRequest == null)
                {
                    return (CancelJoinRequestEnum.RequestNotFound, false);
                }

                _unitOfWork.GroupUserRepository.Remove(joinRequest);
                var result = await _unitOfWork.CompleteAsync();

                if (result > 0)
                {
                    return (CancelJoinRequestEnum.Success, true);
                }

                return (CancelJoinRequestEnum.Failed, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when user {UserId} cancelling join request to group {GroupId}",
                    userId, groupId);
                return (CancelJoinRequestEnum.Failed, false);
            }
        }
      
        public async Task<(GetPendingJoinRequestsEnum, List<GroupUserDto>?)> GetPendingJoinRequestsAsync(
            Guid groupId,
            Guid currentUserId,
            int skip = 0,
            int take = 10)
        {
            try
            {
                var groups = await _unitOfWork.GroupRepository.FindAsyncWithIncludes(
                    g => g.Id == groupId,
                    g => g.GroupUsers
                );

                var group = groups?.FirstOrDefault();
                if (group == null)
                {
                    return (GetPendingJoinRequestsEnum.GroupNotFound, null);
                }
               
                var currentUserRole = group.GroupUsers?.FirstOrDefault(gu => gu.UserId == currentUserId);
                if (currentUserRole == null ||
                    (currentUserRole.RoleName != GroupRole.SuperAdministrator &&
                     currentUserRole.RoleName != GroupRole.Administrator))
                {
                    return (GetPendingJoinRequestsEnum.Unauthorized, null);
                }
              
                var pendingRequests = await _unitOfWork.GroupUserRepository.FindAsyncWithIncludes(
                    gu => gu.GroupId == groupId && gu.RoleName == GroupRole.Pending,
                    gu => gu.User
                );

                if (pendingRequests == null || !pendingRequests.Any())
                {
                    return (GetPendingJoinRequestsEnum.NoRequestsFound, null);
                }

                var paginatedRequests = pendingRequests
                    .OrderBy(gu => gu.JoinedAt)
                    .Skip(skip)
                    .Take(take)
                    .ToList();

                var groupUserDtos = _mapper.Map<List<GroupUserDto>>(paginatedRequests);

                return (GetPendingJoinRequestsEnum.Success, groupUserDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting pending join requests for group {GroupId}", groupId);
                return (GetPendingJoinRequestsEnum.Failed, null);
            }
        }

        public async Task<(LeaveGroupEnum, bool)> LeaveGroupAsync(Guid groupId, Guid userId)
        {
            try
            {
                var group = await _unitOfWork.GroupRepository.GetByIdAsync(groupId);
                if (group == null)
                {
                    return (LeaveGroupEnum.GroupNotFound, false);
                }

                var groupUser = await _unitOfWork.GroupUserRepository
                    .FindFirstAsync(gu => gu.GroupId == groupId && gu.UserId == userId);

                if (groupUser == null)
                {
                    return (LeaveGroupEnum.NotMember, false);
                }

                if (groupUser.RoleName == GroupRole.SuperAdministrator)
                {
                    return (LeaveGroupEnum.CannotLeaveAsOwner, false);
                }

                _unitOfWork.GroupUserRepository.Remove(groupUser);
                var result = await _unitOfWork.CompleteAsync();

                if (result > 0)
                {
                    return (LeaveGroupEnum.LeaveGroupSuccess, true);
                }

                return (LeaveGroupEnum.LeaveGroupFailed, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when user {UserId} leaving group {GroupId}", userId, groupId);
                return (LeaveGroupEnum.LeaveGroupFailed, false);
            }
        }

        public async Task<(GetUserGroupsEnum, List<GroupDto>?)> GetUserGroupsAsync(Guid userId, int skip = 0, int take = 10)
        {
            try
            {
                var groupUsers = await _unitOfWork.GroupUserRepository.FindAsync(
                    gu => gu.UserId == userId && 
                          gu.RoleName != GroupRole.Pending && 
                          gu.RoleName != GroupRole.Inviting
                );

                if (groupUsers == null || !groupUsers.Any())
                {
                    return (GetUserGroupsEnum.NoGroupsFound, null);
                }

                var groupIds = groupUsers.Select(gu => gu.GroupId).ToList();

                var groups = await _unitOfWork.GroupRepository.GetGroupsWithFullDetailsAsync(
                    g => groupIds.Contains(g.Id),
                    skip,
                    take
                );

                if (groups == null || !groups.Any())
                {
                    return (GetUserGroupsEnum.NoGroupsFound, null);
                }

                var groupDtos = _mapper.Map<List<GroupDto>>(groups);

                return (GetUserGroupsEnum.Success, groupDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting groups for user {UserId}", userId);
                return (GetUserGroupsEnum.Failed, null);
            }
        }

        public async Task<(SearchMyGroupsEnum, List<GroupDto>?)> SearchMyGroupsAsync(Guid userId, string searchTerm, int skip = 0, int take = 10)
        {
            try
            {
                var groupUsers = await _unitOfWork.GroupUserRepository.FindAsync(
                    gu => gu.UserId == userId
                );

                if (groupUsers == null || !groupUsers.Any())
                {
                    return (SearchMyGroupsEnum.NoGroupsFound, null);
                }

                var groupIds = groupUsers.Select(gu => gu.GroupId).ToList();

                var searchTermLower = searchTerm.Trim().ToLower();

                var groups = await _unitOfWork.GroupRepository.GetGroupsWithFullDetailsAsync(
                    g => groupIds.Contains(g.Id) && 
                        (g.Name.ToLower().Contains(searchTermLower) || 
                         g.Description.ToLower().Contains(searchTermLower)),
                    skip,
                    take
                );

                if (groups == null || !groups.Any())
                {
                    return (SearchMyGroupsEnum.NoGroupsFound, null);
                }

                var groupDtos = _mapper.Map<List<GroupDto>>(groups);

                return (SearchMyGroupsEnum.Success, groupDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when searching groups for user {UserId} with term {SearchTerm}", userId, searchTerm);
                return (SearchMyGroupsEnum.Failed, null);
            }
        }

        public async Task<(PromoteToAdminEnum, GroupUserDto?)> PromoteToAdminAsync(Guid groupId, Guid targetUserId, Guid currentUserId)
        {
            try
            {           
                if (currentUserId == targetUserId)
                {
                    return (PromoteToAdminEnum.CannotPromoteSelf, null);
                }

                var groups = await _unitOfWork.GroupRepository.FindAsyncWithIncludes(
                    g => g.Id == groupId,
                    g => g.GroupUsers
                );

                var group = groups?.FirstOrDefault();
                if (group == null)
                {
                    return (PromoteToAdminEnum.GroupNotFound, null);
                }

                var currentUserRole = group.GroupUsers?.FirstOrDefault(gu => gu.UserId == currentUserId);
                if (currentUserRole == null || currentUserRole.RoleName != GroupRole.SuperAdministrator)
                {
                    return (PromoteToAdminEnum.Unauthorized, null);
                }

                var targetGroupUser = group.GroupUsers?.FirstOrDefault(gu => gu.UserId == targetUserId);
                if (targetGroupUser == null)
                {
                    return (PromoteToAdminEnum.UserNotMember, null);
                }

                if (targetGroupUser.RoleName == GroupRole.Administrator ||
                    targetGroupUser.RoleName == GroupRole.SuperAdministrator)
                {
                    return (PromoteToAdminEnum.AlreadyAdmin, null);
                }

                var adminCount = group.GroupUsers?.Count(gu =>
                    gu.RoleName == GroupRole.Administrator ||
                    gu.RoleName == GroupRole.SuperAdministrator) ?? 0;

                if (adminCount >= 10)
                {
                    return (PromoteToAdminEnum.MaxAdminReached, null);
                }

                targetGroupUser.RoleName = GroupRole.Administrator;

                _unitOfWork.GroupUserRepository.Update(targetGroupUser);
                var result = await _unitOfWork.CompleteAsync();

                if (result > 0)
                {
                    var updatedGroupUser = await _unitOfWork.GroupUserRepository.FindFirstAsyncWithIncludes(
                        gu => gu.GroupId == groupId && gu.UserId == targetUserId,
                        gu => gu.User
                    );

                    var groupUserDto = _mapper.Map<GroupUserDto>(updatedGroupUser);
                    return (PromoteToAdminEnum.Success, groupUserDto);
                }

                return (PromoteToAdminEnum.Failed, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when promoting user {TargetUserId} to admin in group {GroupId}", targetUserId, groupId);
                return (PromoteToAdminEnum.Failed, null);
            }
        }

        public async Task<(DemoteAdminEnum, GroupUserDto?)> DemoteAdminAsync(Guid groupId, Guid targetUserId, Guid currentUserId)
        {
            try
            {
                if (currentUserId == targetUserId)
                {
                    return (DemoteAdminEnum.CannotDemoteSelf, null);
                }

                var groups = await _unitOfWork.GroupRepository.FindAsyncWithIncludes(
                    g => g.Id == groupId,
                    g => g.GroupUsers
                );

                var group = groups?.FirstOrDefault();
                if (group == null)
                {
                    return (DemoteAdminEnum.GroupNotFound, null);
                }

                var currentUserRole = group.GroupUsers?.FirstOrDefault(gu => gu.UserId == currentUserId);
                if (currentUserRole == null || currentUserRole.RoleName != GroupRole.SuperAdministrator)
                {
                    return (DemoteAdminEnum.Unauthorized, null);
                }

                var targetGroupUser = group.GroupUsers?.FirstOrDefault(gu => gu.UserId == targetUserId);
                if (targetGroupUser == null)
                {
                    return (DemoteAdminEnum.UserNotFound, null);
                }

                if (targetGroupUser.RoleName == GroupRole.User)
                {
                    return (DemoteAdminEnum.UserNotAdmin, null);
                }

                if (targetGroupUser.RoleName == GroupRole.SuperAdministrator)
                {
                    return (DemoteAdminEnum.CannotDemoteSuperAdmin, null);
                }

                targetGroupUser.RoleName = GroupRole.User;

                _unitOfWork.GroupUserRepository.Update(targetGroupUser);
                var result = await _unitOfWork.CompleteAsync();

                if (result > 0)
                {
                    var updatedGroupUser = await _unitOfWork.GroupUserRepository.FindFirstAsyncWithIncludes(
                        gu => gu.GroupId == groupId && gu.UserId == targetUserId,
                        gu => gu.User
                    );

                    var groupUserDto = _mapper.Map<GroupUserDto>(updatedGroupUser);
                    return (DemoteAdminEnum.Success, groupUserDto);
                }

                return (DemoteAdminEnum.Failed, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when demoting admin {TargetUserId} in group {GroupId}", targetUserId, groupId);
                return (DemoteAdminEnum.Failed, null);
            }
        }

        public async Task<(KickMemberEnum, bool)> KickMemberAsync(Guid groupId, Guid targetUserId, Guid currentUserId)
        {
            try
            {
                if (currentUserId == targetUserId)
                {
                    return (KickMemberEnum.CannotKickSelf, false);
                }

                var groups = await _unitOfWork.GroupRepository.FindAsyncWithIncludes(
                    g => g.Id == groupId,
                    g => g.GroupUsers
                );

                var group = groups?.FirstOrDefault();
                if (group == null)
                {
                    return (KickMemberEnum.GroupNotFound, false);
                }

                var currentUserRole = group.GroupUsers?.FirstOrDefault(gu => gu.UserId == currentUserId);
                if (currentUserRole == null)
                {
                    return (KickMemberEnum.Unauthorized, false);
                }

                if (currentUserRole.RoleName != GroupRole.SuperAdministrator &&
                    currentUserRole.RoleName != GroupRole.Administrator)
                {
                    return (KickMemberEnum.Unauthorized, false);
                }
          
                var targetGroupUser = group.GroupUsers?.FirstOrDefault(gu => gu.UserId == targetUserId);
                if (targetGroupUser == null)
                {
                    return (KickMemberEnum.TargetUserNotMember, false);
                }
             
                if (targetGroupUser.RoleName == GroupRole.SuperAdministrator)
                {
                    return (KickMemberEnum.CannotKickSuperAdmin, false);
                }
                
                if (currentUserRole.RoleName == GroupRole.Administrator &&
                    targetGroupUser.RoleName == GroupRole.Administrator)
                {
                    return (KickMemberEnum.AdminCannotKickAdmin, false);
                }

                _unitOfWork.GroupUserRepository.Remove(targetGroupUser);
                var result = await _unitOfWork.CompleteAsync();

                if (result > 0)
                {
                    return (KickMemberEnum.Success, true);
                }

                return (KickMemberEnum.Failed, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when kicking user {TargetUserId} from group {GroupId}", targetUserId, groupId);
                return (KickMemberEnum.Failed, false);
            }
        }

        public async Task<(InviteMemberEnum, bool)> InviteMemberAsync(Guid groupId, Guid targetUserId, Guid inviterId)
        {
            try
            {
                if (inviterId == targetUserId)
                {
                    return (InviteMemberEnum.CannotInviteSelf, false);
                }

                var groups = await _unitOfWork.GroupRepository.FindAsyncWithIncludes(
                    g => g.Id == groupId,
                    g => g.GroupUsers
                );

                var group = groups?.FirstOrDefault();
                if (group == null)
                {
                    return (InviteMemberEnum.GroupNotFound, false);
                }

                var inviterMembership = group.GroupUsers?.FirstOrDefault(
                    gu => gu.UserId == inviterId &&
                          gu.RoleName != GroupRole.Pending &&
                          gu.RoleName != GroupRole.Inviting
                );

                if (inviterMembership == null)
                {
                    return (InviteMemberEnum.InviterNotMember, false);
                }

                var targetUser = await _unitOfWork.UserRepository.GetByIdAsync(targetUserId);
                if (targetUser == null)
                {
                    return (InviteMemberEnum.TargetUserNotFound, false);
                }

                var existingMembership = group.GroupUsers?.FirstOrDefault(gu => gu.UserId == targetUserId);

                if (existingMembership != null)
                {
                    if (existingMembership.RoleName != GroupRole.Inviting &&
                        existingMembership.RoleName != GroupRole.Pending)
                    {
                        return (InviteMemberEnum.AlreadyMember, false);
                    }

                    if (existingMembership.RoleName == GroupRole.Inviting)
                    {
                        return (InviteMemberEnum.AlreadyInvited, false);
                    }
                }

                var invitation = new GroupUser
                {
                    UserId = targetUserId,
                    GroupId = groupId,
                    RoleName = GroupRole.Inviting,
                    JoinedAt = DateTime.UtcNow
                };

                _unitOfWork.GroupUserRepository.Add(invitation);
                var result = await _unitOfWork.CompleteAsync();

                if (result > 0)
                {
                    try
                    {                    
                        var inviter = await _unitOfWork.UserRepository.GetByIdAsync(inviterId);
                        if (inviter != null)
                        {
                            using (var scope = _serviceProvider.CreateScope())
                            {
                                var notificationDataBuilder = scope.ServiceProvider.GetRequiredService<INotificationDataBuilder>();
                                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                                var notificationData = await notificationDataBuilder.BuilderDataForGroupInvite(
                                    group,
                                    inviter,
                                    targetUser
                                );

                                if (notificationData != null)
                                {
                                    await notificationService.ProcessAndSendNotiForGroupInvite(
                                        NotificationType.GroupInvite,
                                        notificationData,
                                        $"/group/{groupId}",
                                        targetUserId,
                                        groupId
                                    );
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error sending group invite notification");
                    }

                    return (InviteMemberEnum.Success, true);
                }

                return (InviteMemberEnum.Failed, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when inviting user {TargetUserId} to group {GroupId}",
                    targetUserId, groupId);
                return (InviteMemberEnum.Failed, false);
            }
        }

        public async Task<(AcceptGroupInviteEnum, GroupDto?)> AcceptGroupInviteAsync(Guid groupId, Guid userId)
        {
            try
            {
                var groups = await _unitOfWork.GroupRepository.FindAsyncWithIncludes(
                    g => g.Id == groupId,
                    g => g.GroupUsers
                );

                var group = groups?.FirstOrDefault();
                if (group == null)
                {
                    return (AcceptGroupInviteEnum.GroupNotFound, null);
                }

                var invitation = group.GroupUsers?.FirstOrDefault(
                    gu => gu.UserId == userId && gu.RoleName == GroupRole.Inviting
                );

                if (invitation == null)
                {
                    var existingMember = group.GroupUsers?.FirstOrDefault(
                        gu => gu.UserId == userId &&
                              gu.RoleName != GroupRole.Inviting &&
                              gu.RoleName != GroupRole.Pending
                    );

                    if (existingMember != null)
                    {
                        return (AcceptGroupInviteEnum.AlreadyMember, null);
                    }

                    return (AcceptGroupInviteEnum.InvitationNotFound, null);
                }

                invitation.RoleName = GroupRole.User;
                invitation.JoinedAt = DateTime.UtcNow;

                _unitOfWork.GroupUserRepository.Update(invitation);
                var result = await _unitOfWork.CompleteAsync();

                if (result > 0)
                {
                    var updatedGroup = await _unitOfWork.GroupRepository.GetGroupWithFullDetailsByIdAsync(groupId);
                    var groupDto = _mapper.Map<GroupDto>(updatedGroup);

                    return (AcceptGroupInviteEnum.Success, groupDto);
                }

                return (AcceptGroupInviteEnum.Failed, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when user {UserId} accepting invite to group {GroupId}",
                    userId, groupId);
                return (AcceptGroupInviteEnum.Failed, null);
            }
        }

        public async Task<(RejectGroupInviteEnum, bool)> RejectGroupInviteAsync(Guid groupId, Guid userId)
        {
            try
            {
                var group = await _unitOfWork.GroupRepository.GetByIdAsync(groupId);
                if (group == null)
                {
                    return (RejectGroupInviteEnum.GroupNotFound, false);
                }

                var invitation = await _unitOfWork.GroupUserRepository.FindFirstAsync(
                    gu => gu.GroupId == groupId &&
                          gu.UserId == userId &&
                          gu.RoleName == GroupRole.Inviting
                );

                if (invitation == null)
                {
                    return (RejectGroupInviteEnum.InvitationNotFound, false);
                }

                _unitOfWork.GroupUserRepository.Remove(invitation);
                var result = await _unitOfWork.CompleteAsync();

                if (result > 0)
                {
                    return (RejectGroupInviteEnum.Success, true);
                }

                return (RejectGroupInviteEnum.Failed, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when user {UserId} rejecting invite to group {GroupId}",
                    userId, groupId);
                return (RejectGroupInviteEnum.Failed, false);
            }
        }

        public async Task<(GetMyGroupInvitationsEnum, List<GroupInvitationDto>?)> GetMyGroupInvitationsAsync(
            Guid userId,
            int skip = 0,
            int take = 10)
        {
            try
            {
                var invitations = await _unitOfWork.GroupUserRepository.FindAsyncWithIncludes(
                    gu => gu.UserId == userId && gu.RoleName == GroupRole.Inviting,
                    gu => gu.Group,
                    gu => gu.Group.GroupUsers
                );

                if (invitations == null || !invitations.Any())
                {
                    return (GetMyGroupInvitationsEnum.NoInvitationsFound, null);
                }

                var invitationDtos = new List<GroupInvitationDto>();

                foreach (var invitation in invitations.OrderByDescending(i => i.JoinedAt).Skip(skip).Take(take))
                {
                    if (invitation.Group == null) continue;

                    var dto = new GroupInvitationDto
                    {
                        GroupId = invitation.GroupId,
                        GroupName = invitation.Group.Name,
                        GroupDescription = invitation.Group.Description,
                        GroupImageUrl = invitation.Group.ImageUrl,
                        IsPublic = invitation.Group.IsPublic == 1,
                        MemberCount = invitation.Group.GroupUsers?.Count(gu =>
                            gu.RoleName != GroupRole.Pending &&
                            gu.RoleName != GroupRole.Inviting) ?? 0,
                        InvitedAt = invitation.JoinedAt
                    };

                    invitationDtos.Add(dto);
                }

                return (GetMyGroupInvitationsEnum.Success, invitationDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting group invitations for user {UserId}", userId);
                return (GetMyGroupInvitationsEnum.Failed, null);
            }
        }
    }
}