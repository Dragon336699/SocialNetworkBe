using AutoMapper;
using Domain.Contracts.Requests.Group;
using Domain.Contracts.Responses.Group;
using Domain.Entities;
using Domain.Enum.Group.Functions;
using Domain.Enum.Group.Types;
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

        public GroupService(
            IUnitOfWork unitOfWork,
            ILogger<GroupService> logger,
            IMapper mapper,
            IUploadService uploadService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _uploadService = uploadService;
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
                    RoleName = GroupRole.Administrator,
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
                    var isMember = group.GroupUsers?.Any(gu => gu.UserId == userId) ?? false;
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
                if (groupUser == null || groupUser.RoleName != GroupRole.Administrator)
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
                if (groupUser == null || groupUser.RoleName != GroupRole.Administrator)
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
                var group = await _unitOfWork.GroupRepository.GetByIdAsync(groupId);
                if (group == null)
                {
                    return (JoinGroupEnum.GroupNotFound, false);
                }

                var existingMember = await _unitOfWork.GroupUserRepository
                    .FindFirstAsync(gu => gu.GroupId == groupId && gu.UserId == userId);

                if (existingMember != null)
                {
                    return (JoinGroupEnum.AlreadyMember, false);
                }

                var groupUser = new GroupUser
                {
                    UserId = userId,
                    GroupId = groupId,
                    RoleName = GroupRole.User,
                    JoinedAt = DateTime.UtcNow
                };

                _unitOfWork.GroupUserRepository.Add(groupUser);
                var result = await _unitOfWork.CompleteAsync();

                if (result > 0)
                {
                    return (JoinGroupEnum.JoinGroupSuccess, true);
                }

                return (JoinGroupEnum.JoinGroupFailed, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when user {UserId} joining group {GroupId}", userId, groupId);
                return (JoinGroupEnum.JoinGroupFailed, false);
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

                if (groupUser.RoleName == GroupRole.Administrator)
                {
                    return (LeaveGroupEnum.CannotLeaveAsAdmin, false);
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
                    gu => gu.UserId == userId
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
    }
}