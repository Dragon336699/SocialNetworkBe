using Domain.Contracts.Requests.Group;
using Domain.Contracts.Responses.Group;
using Domain.Enum.Group.Functions;
using Domain.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SocialNetworkBe.Controllers
{
    [ApiController]
    [Route("api/v1/group")]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateGroup([FromForm] CreateGroupRequest request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var (status, groupId) = await _groupService.CreateGroupAsync(request, userId);

                return status switch
                {
                    CreateGroupEnum.UserNotFound => BadRequest(new CreateGroupResponse { Message = status.GetMessage() }),
                    CreateGroupEnum.InvalidName => BadRequest(new CreateGroupResponse { Message = status.GetMessage() }),
                    CreateGroupEnum.InvalidDescription => BadRequest(new CreateGroupResponse { Message = status.GetMessage() }),
                    CreateGroupEnum.InvalidImageFormat => BadRequest(new CreateGroupResponse { Message = status.GetMessage() }),
                    CreateGroupEnum.FileTooLarge => BadRequest(new CreateGroupResponse { Message = status.GetMessage() }),
                    CreateGroupEnum.ImageUploadFailed => StatusCode(500, new CreateGroupResponse { Message = status.GetMessage() }),
                    CreateGroupEnum.CreateGroupSuccess => Ok(new CreateGroupResponse
                    {
                        Message = status.GetMessage(),
                        GroupId = groupId
                    }),
                    CreateGroupEnum.CreateGroupFailed => StatusCode(500, new CreateGroupResponse { Message = status.GetMessage() }),
                    _ => StatusCode(500, new CreateGroupResponse { Message = "Unknown error occurred" })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new CreateGroupResponse { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllGroups([FromQuery] int skip = 0, [FromQuery] int take = 10)
        {
            try
            {
                var (status, groups) = await _groupService.GetAllGroupsAsync(skip, take);

                return status switch
                {
                    GetAllGroupsEnum.Success => Ok(new GetAllGroupsResponse
                    {
                        Message = status.GetMessage(),
                        Groups = groups,
                        TotalCount = groups?.Count ?? 0
                    }),
                    GetAllGroupsEnum.NoGroupsFound => Ok(new GetAllGroupsResponse
                    {
                        Message = status.GetMessage(),
                        Groups = new List<GroupDto>(),
                        TotalCount = 0
                    }),
                    GetAllGroupsEnum.Failed => StatusCode(500, new GetAllGroupsResponse
                    {
                        Message = status.GetMessage(),
                        Groups = null,
                        TotalCount = 0
                    }),
                    _ => StatusCode(500, new GetAllGroupsResponse
                    {
                        Message = "Unknown error occurred",
                        Groups = null,
                        TotalCount = 0
                    })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GetAllGroupsResponse
                {
                    Message = ex.Message,
                    Groups = null,
                    TotalCount = 0
                });
            }
        }

        [Authorize]
        [HttpGet("{groupId}")]
        public async Task<IActionResult> GetGroupById(Guid groupId)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var (status, groupDto) = await _groupService.GetGroupByIdAsync(groupId, userId);

                return status switch
                {
                    GetGroupByIdEnum.Success => Ok(new GetGroupByIdResponse
                    {
                        Message = status.GetMessage(),
                        Group = groupDto
                    }),
                    GetGroupByIdEnum.GroupNotFound => NotFound(new GetGroupByIdResponse { Message = status.GetMessage() }),
                    GetGroupByIdEnum.Unauthorized => StatusCode(403, new GetGroupByIdResponse { Message = status.GetMessage() }),
                    GetGroupByIdEnum.Failed => StatusCode(500, new GetGroupByIdResponse { Message = status.GetMessage() }),
                    _ => StatusCode(500, new GetGroupByIdResponse { Message = "Unknown error occurred" })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GetGroupByIdResponse { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("{groupId}")]
        public async Task<IActionResult> UpdateGroup([FromForm] UpdateGroupRequest request, Guid groupId)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var (status, groupDto) = await _groupService.UpdateGroupAsync(groupId, request, userId);

                return status switch
                {
                    UpdateGroupEnum.GroupNotFound => NotFound(new UpdateGroupResponse { Message = status.GetMessage() }),
                    UpdateGroupEnum.Unauthorized => StatusCode(403, new UpdateGroupResponse { Message = status.GetMessage() }),
                    UpdateGroupEnum.InvalidName => BadRequest(new UpdateGroupResponse { Message = status.GetMessage() }),
                    UpdateGroupEnum.InvalidDescription => BadRequest(new UpdateGroupResponse { Message = status.GetMessage() }),
                    UpdateGroupEnum.InvalidImageFormat => BadRequest(new UpdateGroupResponse { Message = status.GetMessage() }),
                    UpdateGroupEnum.FileTooLarge => BadRequest(new UpdateGroupResponse { Message = status.GetMessage() }),
                    UpdateGroupEnum.ImageUploadFailed => StatusCode(500, new UpdateGroupResponse { Message = status.GetMessage() }),
                    UpdateGroupEnum.UpdateGroupSuccess => Ok(new UpdateGroupResponse
                    {
                        Message = status.GetMessage(),
                        Group = groupDto
                    }),
                    UpdateGroupEnum.UpdateGroupFailed => StatusCode(500, new UpdateGroupResponse { Message = status.GetMessage() }),
                    _ => StatusCode(500, new UpdateGroupResponse { Message = "Unknown error occurred" })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new UpdateGroupResponse { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("{groupId}")]
        public async Task<IActionResult> DeleteGroup(Guid groupId)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var (status, result) = await _groupService.DeleteGroupAsync(groupId, userId);

                return status switch
                {
                    DeleteGroupEnum.GroupNotFound => NotFound(new DeleteGroupResponse { Message = status.GetMessage() }),
                    DeleteGroupEnum.Unauthorized => StatusCode(403, new DeleteGroupResponse { Message = status.GetMessage() }),
                    DeleteGroupEnum.DeleteGroupSuccess => Ok(new DeleteGroupResponse { Message = status.GetMessage() }),
                    DeleteGroupEnum.DeleteGroupFailed => StatusCode(500, new DeleteGroupResponse { Message = status.GetMessage() }),
                    _ => StatusCode(500, new DeleteGroupResponse { Message = "Unknown error occurred" })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new DeleteGroupResponse { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("{groupId}/join")]
        public async Task<IActionResult> JoinGroup(Guid groupId)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var (status, result) = await _groupService.JoinGroupAsync(groupId, userId);

                return status switch
                {
                    JoinGroupEnum.GroupNotFound => NotFound(new JoinGroupResponse { Message = status.GetMessage() }),
                    JoinGroupEnum.AlreadyMember => BadRequest(new JoinGroupResponse { Message = status.GetMessage() }),
                    JoinGroupEnum.JoinGroupSuccess => Ok(new JoinGroupResponse { Message = status.GetMessage() }),
                    JoinGroupEnum.JoinGroupFailed => StatusCode(500, new JoinGroupResponse { Message = status.GetMessage() }),
                    _ => StatusCode(500, new JoinGroupResponse { Message = "Unknown error occurred" })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new JoinGroupResponse { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("{groupId}/leave")]
        public async Task<IActionResult> LeaveGroup(Guid groupId)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var (status, result) = await _groupService.LeaveGroupAsync(groupId, userId);

                return status switch
                {
                    LeaveGroupEnum.GroupNotFound => NotFound(new LeaveGroupResponse { Message = status.GetMessage() }),
                    LeaveGroupEnum.NotMember => BadRequest(new LeaveGroupResponse { Message = status.GetMessage() }),
                    LeaveGroupEnum.CannotLeaveAsOwner => BadRequest(new LeaveGroupResponse { Message = status.GetMessage() }),
                    LeaveGroupEnum.LeaveGroupSuccess => Ok(new LeaveGroupResponse { Message = status.GetMessage() }),
                    LeaveGroupEnum.LeaveGroupFailed => StatusCode(500, new LeaveGroupResponse { Message = status.GetMessage() }),
                    _ => StatusCode(500, new LeaveGroupResponse { Message = "Unknown error occurred" })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new LeaveGroupResponse { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("my-groups")]
        public async Task<IActionResult> GetMyGroups([FromQuery] int skip = 0, [FromQuery] int take = 10)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var (status, groups) = await _groupService.GetUserGroupsAsync(userId, skip, take);

                return status switch
                {
                    GetUserGroupsEnum.Success => Ok(new GetAllGroupsResponse
                    {
                        Message = status.GetMessage(),
                        Groups = groups,
                        TotalCount = groups?.Count ?? 0
                    }),
                    GetUserGroupsEnum.NoGroupsFound => Ok(new GetAllGroupsResponse
                    {
                        Message = status.GetMessage(),
                        Groups = new List<GroupDto>(),
                        TotalCount = 0
                    }),
                    GetUserGroupsEnum.Failed => StatusCode(500, new GetAllGroupsResponse
                    {
                        Message = status.GetMessage(),
                        Groups = null,
                        TotalCount = 0
                    }),
                    _ => StatusCode(500, new GetAllGroupsResponse
                    {
                        Message = "Unknown error occurred",
                        Groups = null,
                        TotalCount = 0
                    })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GetAllGroupsResponse
                {
                    Message = ex.Message,
                    Groups = null,
                    TotalCount = 0
                });
            }
        }

        [Authorize]
        [HttpPost("{groupId}/promote-admin")]
        public async Task<IActionResult> PromoteToAdmin(Guid groupId, [FromBody] PromoteToAdminRequest request)
        {
            try
            {
                var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var (status, groupUserDto) = await _groupService.PromoteToAdminAsync(groupId, request.TargetUserId, currentUserId);

                return status switch
                {
                    PromoteToAdminEnum.Success => Ok(new PromoteToAdminResponse
                    {
                        Message = status.GetMessage(),
                        GroupUser = groupUserDto
                    }),
                    PromoteToAdminEnum.GroupNotFound => NotFound(new PromoteToAdminResponse { Message = status.GetMessage() }),
                    PromoteToAdminEnum.UserNotFound => NotFound(new PromoteToAdminResponse { Message = status.GetMessage() }),
                    PromoteToAdminEnum.UserNotMember => BadRequest(new PromoteToAdminResponse { Message = status.GetMessage() }),
                    PromoteToAdminEnum.AlreadyAdmin => BadRequest(new PromoteToAdminResponse { Message = status.GetMessage() }),
                    PromoteToAdminEnum.MaxAdminReached => BadRequest(new PromoteToAdminResponse { Message = status.GetMessage() }),
                    PromoteToAdminEnum.CannotPromoteSelf => BadRequest(new PromoteToAdminResponse { Message = status.GetMessage() }),
                    PromoteToAdminEnum.Failed => StatusCode(500, new PromoteToAdminResponse { Message = status.GetMessage() }),
                    _ => StatusCode(500, new PromoteToAdminResponse { Message = "Unknown error occurred" })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new PromoteToAdminResponse { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("{groupId}/demote-admin")]
        public async Task<IActionResult> DemoteAdmin(Guid groupId, [FromBody] PromoteToAdminRequest request)
        {
            try
            {
                var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var (status, groupUserDto) = await _groupService.DemoteAdminAsync(groupId, request.TargetUserId, currentUserId);

                return status switch
                {
                    DemoteAdminEnum.Success => Ok(new DemoteAdminResponse
                    {
                        Message = status.GetMessage(),
                        GroupUser = groupUserDto
                    }),
                    DemoteAdminEnum.GroupNotFound => NotFound(new DemoteAdminResponse { Message = status.GetMessage() }),
                    DemoteAdminEnum.UserNotFound => NotFound(new DemoteAdminResponse { Message = status.GetMessage() }),
                    DemoteAdminEnum.UserNotAdmin => BadRequest(new DemoteAdminResponse { Message = status.GetMessage() }),
                    DemoteAdminEnum.CannotDemoteSuperAdmin => BadRequest(new DemoteAdminResponse { Message = status.GetMessage() }),
                    DemoteAdminEnum.CannotDemoteSelf => BadRequest(new DemoteAdminResponse { Message = status.GetMessage() }),
                    DemoteAdminEnum.Failed => StatusCode(500, new DemoteAdminResponse { Message = status.GetMessage() }),
                    _ => StatusCode(500, new DemoteAdminResponse { Message = "Unknown error occurred" })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new DemoteAdminResponse { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("{groupId}/kick-member")]
        public async Task<IActionResult> KickMember(Guid groupId, [FromBody] KickMemberRequest request)
        {
            try
            {
                var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var (status, result) = await _groupService.KickMemberAsync(groupId, request.TargetUserId, currentUserId);

                return status switch
                {
                    KickMemberEnum.Success => Ok(new KickMemberResponse { Message = status.GetMessage(), Success = true }),
                    KickMemberEnum.GroupNotFound => NotFound(new KickMemberResponse { Message = status.GetMessage(), Success = false }),
                    KickMemberEnum.UserNotFound => NotFound(new KickMemberResponse { Message = status.GetMessage(), Success = false }),
                    KickMemberEnum.TargetUserNotMember => BadRequest(new KickMemberResponse { Message = status.GetMessage(), Success = false }),
                    KickMemberEnum.Unauthorized => StatusCode(403, new KickMemberResponse { Message = status.GetMessage(), Success = false }),
                    KickMemberEnum.CannotKickSelf => BadRequest(new KickMemberResponse { Message = status.GetMessage(), Success = false }),
                    KickMemberEnum.CannotKickSuperAdmin => BadRequest(new KickMemberResponse { Message = status.GetMessage(), Success = false }),
                    KickMemberEnum.AdminCannotKickAdmin => StatusCode(403, new KickMemberResponse { Message = status.GetMessage(), Success = false }),
                    KickMemberEnum.Failed => StatusCode(500, new KickMemberResponse { Message = status.GetMessage(), Success = false }),
                    _ => StatusCode(500, new KickMemberResponse { Message = "Unknown error occurred", Success = false })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new KickMemberResponse { Message = ex.Message, Success = false });
            }
        }
    }
}