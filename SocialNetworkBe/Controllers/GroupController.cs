using Domain.Contracts.Requests.Group;
using Domain.Contracts.Responses.Group;
using Domain.Entities;
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
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var (status, groups) = await _groupService.GetAllGroupsAsync(userId, skip, take);

                return status switch
                {
                    GetAllGroupsEnum.Success => Ok(new GetAllGroupsResponse { Message = status.GetMessage(), Groups = groups, TotalCount = groups?.Count ?? 0 }),
                    GetAllGroupsEnum.NoGroupsFound => Ok(new GetAllGroupsResponse { Message = status.GetMessage(), Groups = new List<GroupDto>(), TotalCount = 0 }),
                    GetAllGroupsEnum.Failed => StatusCode(500, new GetAllGroupsResponse { Message = status.GetMessage(), Groups = null, TotalCount = 0 }),
                    _ => StatusCode(500, new GetAllGroupsResponse { Message = "Unknown error occurred", Groups = null, TotalCount = 0 })
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
                    JoinGroupEnum.AlreadyRequested => BadRequest(new JoinGroupResponse { Message = status.GetMessage() }),
                    JoinGroupEnum.UserBanned => StatusCode(403, new JoinGroupResponse { Message = status.GetMessage() }),
                    JoinGroupEnum.JoinRequestSent => Ok(new JoinGroupResponse { Message = status.GetMessage() }),
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
        [HttpPost("{groupId}/approve-join-request")]
        public async Task<IActionResult> ApproveJoinRequest(Guid groupId, [FromBody] ApproveJoinRequestRequest request)
        {
            try
            {
                var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var (status, groupUserDto) = await _groupService.ApproveJoinRequestAsync(groupId, request.TargetUserId, currentUserId);

                return status switch
                {
                    ApproveJoinRequestEnum.Success => Ok(new ApproveJoinRequestResponse { Message = status.GetMessage(), GroupUser = groupUserDto }),
                    ApproveJoinRequestEnum.GroupNotFound => NotFound(new ApproveJoinRequestResponse { Message = status.GetMessage() }),
                    ApproveJoinRequestEnum.UserNotFound => NotFound(new ApproveJoinRequestResponse { Message = status.GetMessage() }),
                    ApproveJoinRequestEnum.RequestNotFound => NotFound(new ApproveJoinRequestResponse { Message = status.GetMessage() }),
                    ApproveJoinRequestEnum.AlreadyMember => BadRequest(new ApproveJoinRequestResponse { Message = status.GetMessage() }),
                    ApproveJoinRequestEnum.Unauthorized => StatusCode(403, new ApproveJoinRequestResponse { Message = status.GetMessage() }),
                    ApproveJoinRequestEnum.Failed => StatusCode(500, new ApproveJoinRequestResponse { Message = status.GetMessage() }),
                    _ => StatusCode(500, new ApproveJoinRequestResponse { Message = "Unknown error occurred" })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApproveJoinRequestResponse { Message = ex.Message });
            }
        }
    
        [Authorize]
        [HttpPost("{groupId}/reject-join-request")]
        public async Task<IActionResult> RejectJoinRequest(Guid groupId, [FromBody] RejectJoinRequestRequest request)
        {
            try
            {
                var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var (status, result) = await _groupService.RejectJoinRequestAsync(groupId, request.TargetUserId, currentUserId);

                return status switch
                {
                    RejectJoinRequestEnum.Success => Ok(new RejectJoinRequestResponse { Message = status.GetMessage() }),
                    RejectJoinRequestEnum.GroupNotFound => NotFound(new RejectJoinRequestResponse { Message = status.GetMessage() }),
                    RejectJoinRequestEnum.UserNotFound => NotFound(new RejectJoinRequestResponse { Message = status.GetMessage() }),
                    RejectJoinRequestEnum.RequestNotFound => NotFound(new RejectJoinRequestResponse { Message = status.GetMessage() }),
                    RejectJoinRequestEnum.Unauthorized => StatusCode(403, new RejectJoinRequestResponse { Message = status.GetMessage() }),
                    RejectJoinRequestEnum.Failed => StatusCode(500, new RejectJoinRequestResponse { Message = status.GetMessage() }),
                    _ => StatusCode(500, new RejectJoinRequestResponse { Message = "Unknown error occurred" })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new RejectJoinRequestResponse { Message = ex.Message });
            }
        }
   
        [Authorize]
        [HttpPost("{groupId}/cancel-join-request")]
        public async Task<IActionResult> CancelJoinRequest(Guid groupId)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var (status, result) = await _groupService.CancelJoinRequestAsync(groupId, userId);

                return status switch
                {
                    CancelJoinRequestEnum.Success => Ok(new CancelJoinRequestResponse { Message = status.GetMessage() }),
                    CancelJoinRequestEnum.GroupNotFound => NotFound(new CancelJoinRequestResponse { Message = status.GetMessage() }),
                    CancelJoinRequestEnum.RequestNotFound => NotFound(new CancelJoinRequestResponse { Message = status.GetMessage() }),
                    CancelJoinRequestEnum.Failed => StatusCode(500, new CancelJoinRequestResponse { Message = status.GetMessage() }),
                    _ => StatusCode(500, new CancelJoinRequestResponse { Message = "Unknown error occurred" })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new CancelJoinRequestResponse { Message = ex.Message });
            }
        }
       
        [Authorize]
        [HttpGet("{groupId}/pending-join-requests")]
        public async Task<IActionResult> GetPendingJoinRequests(Guid groupId, [FromQuery] int skip = 0, [FromQuery] int take = 10)
        {
            try
            {
                var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var (status, pendingRequests) = await _groupService.GetPendingJoinRequestsAsync(groupId, currentUserId, skip, take);

                return status switch
                {
                    GetPendingJoinRequestsEnum.Success => Ok(new GetPendingJoinRequestsResponse { Message = status.GetMessage(), PendingRequests = pendingRequests, TotalCount = pendingRequests?.Count ?? 0 }),
                    GetPendingJoinRequestsEnum.GroupNotFound => NotFound(new GetPendingJoinRequestsResponse { Message = status.GetMessage(), PendingRequests = null, TotalCount = 0 }),
                    GetPendingJoinRequestsEnum.Unauthorized => StatusCode(403, new GetPendingJoinRequestsResponse { Message = status.GetMessage(), PendingRequests = null, TotalCount = 0 }),
                    GetPendingJoinRequestsEnum.NoRequestsFound => Ok(new GetPendingJoinRequestsResponse { Message = status.GetMessage(), PendingRequests = new List<GroupUserDto>(), TotalCount = 0 }),
                    GetPendingJoinRequestsEnum.Failed => StatusCode(500, new GetPendingJoinRequestsResponse { Message = status.GetMessage(), PendingRequests = null, TotalCount = 0 }),
                    _ => StatusCode(500, new GetPendingJoinRequestsResponse { Message = "Unknown error occurred", PendingRequests = null, TotalCount = 0 })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GetPendingJoinRequestsResponse
                {
                    Message = ex.Message,
                    PendingRequests = null,
                    TotalCount = 0
                });
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
        [HttpGet("my-groups/search")]
        public async Task<IActionResult> SearchMyGroups([FromQuery] string searchTerm, [FromQuery] int skip = 0, [FromQuery] int take = 10)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var (status, groups) = await _groupService.SearchMyGroupsAsync(userId, searchTerm, skip, take);

                return status switch
                {
                    SearchMyGroupsEnum.Success => Ok(new SearchMyGroupsResponse
                    {
                        Message = status.GetMessage(),
                        Groups = groups,
                        TotalCount = groups?.Count ?? 0
                    }),
                    SearchMyGroupsEnum.NoGroupsFound => Ok(new SearchMyGroupsResponse
                    {
                        Message = status.GetMessage(),
                        Groups = new List<GroupDto>(),
                        TotalCount = 0
                    }),
                    SearchMyGroupsEnum.Failed => StatusCode(500, new SearchMyGroupsResponse
                    {
                        Message = status.GetMessage(),
                        Groups = null,
                        TotalCount = 0
                    }),
                    _ => StatusCode(500, new SearchMyGroupsResponse
                    {
                        Message = "Unknown error occurred",
                        Groups = null,
                        TotalCount = 0
                    })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new SearchMyGroupsResponse
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

        [Authorize]
        [HttpPost("{groupId}/invite-member")]
        public async Task<IActionResult> InviteMember(Guid groupId, [FromBody] InviteMemberRequest request)
        {
            try
            {
                var inviterId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var (status, result) = await _groupService.InviteMemberAsync(groupId, request.TargetUserId, inviterId);

                return status switch
                {
                    InviteMemberEnum.Success => Ok(new InviteMemberResponse { Message = status.GetMessage() }),
                    InviteMemberEnum.GroupNotFound => NotFound(new InviteMemberResponse { Message = status.GetMessage() }),
                    InviteMemberEnum.InviterNotMember => BadRequest(new InviteMemberResponse { Message = status.GetMessage() }),
                    InviteMemberEnum.TargetUserNotFound => NotFound(new InviteMemberResponse { Message = status.GetMessage() }),
                    InviteMemberEnum.AlreadyMember => BadRequest(new InviteMemberResponse { Message = status.GetMessage() }),
                    InviteMemberEnum.AlreadyInvited => BadRequest(new InviteMemberResponse { Message = status.GetMessage() }),
                    InviteMemberEnum.CannotInviteSelf => BadRequest(new InviteMemberResponse { Message = status.GetMessage() }),
                    InviteMemberEnum.Failed => StatusCode(500, new InviteMemberResponse { Message = status.GetMessage() }),
                    _ => StatusCode(500, new InviteMemberResponse { Message = "Unknown error occurred" })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new InviteMemberResponse { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("{groupId}/accept-invite")]
        public async Task<IActionResult> AcceptGroupInvite(Guid groupId)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var (status, groupDto) = await _groupService.AcceptGroupInviteAsync(groupId, userId);

                return status switch
                {
                    AcceptGroupInviteEnum.Success => Ok(new AcceptGroupInviteResponse
                    {
                        Message = status.GetMessage(),
                        Group = groupDto
                    }),
                    AcceptGroupInviteEnum.GroupNotFound => NotFound(new AcceptGroupInviteResponse { Message = status.GetMessage() }),
                    AcceptGroupInviteEnum.InvitationNotFound => NotFound(new AcceptGroupInviteResponse { Message = status.GetMessage() }),
                    AcceptGroupInviteEnum.AlreadyMember => BadRequest(new AcceptGroupInviteResponse { Message = status.GetMessage() }),
                    AcceptGroupInviteEnum.Failed => StatusCode(500, new AcceptGroupInviteResponse { Message = status.GetMessage() }),
                    _ => StatusCode(500, new AcceptGroupInviteResponse { Message = "Unknown error occurred" })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new AcceptGroupInviteResponse { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("{groupId}/reject-invite")]
        public async Task<IActionResult> RejectGroupInvite(Guid groupId)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var (status, result) = await _groupService.RejectGroupInviteAsync(groupId, userId);

                return status switch
                {
                    RejectGroupInviteEnum.Success => Ok(new RejectGroupInviteResponse { Message = status.GetMessage() }),
                    RejectGroupInviteEnum.GroupNotFound => NotFound(new RejectGroupInviteResponse { Message = status.GetMessage() }),
                    RejectGroupInviteEnum.InvitationNotFound => NotFound(new RejectGroupInviteResponse { Message = status.GetMessage() }),
                    RejectGroupInviteEnum.Failed => StatusCode(500, new RejectGroupInviteResponse { Message = status.GetMessage() }),
                    _ => StatusCode(500, new RejectGroupInviteResponse { Message = "Unknown error occurred" })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new RejectGroupInviteResponse { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("my-invitations")]
        public async Task<IActionResult> GetMyGroupInvitations([FromQuery] int skip = 0, [FromQuery] int take = 10)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var (status, invitations) = await _groupService.GetMyGroupInvitationsAsync(userId, skip, take);

                return status switch
                {
                    GetMyGroupInvitationsEnum.Success => Ok(new GetMyGroupInvitationsResponse
                    {
                        Message = status.GetMessage(),
                        Invitations = invitations,
                        TotalCount = invitations?.Count ?? 0
                    }),
                    GetMyGroupInvitationsEnum.NoInvitationsFound => Ok(new GetMyGroupInvitationsResponse
                    {
                        Message = status.GetMessage(),
                        Invitations = new List<GroupInvitationDto>(),
                        TotalCount = 0
                    }),
                    GetMyGroupInvitationsEnum.Failed => StatusCode(500, new GetMyGroupInvitationsResponse
                    {
                        Message = status.GetMessage(),
                        Invitations = null,
                        TotalCount = 0
                    }),
                    _ => StatusCode(500, new GetMyGroupInvitationsResponse
                    {
                        Message = "Unknown error occurred",
                        Invitations = null,
                        TotalCount = 0
                    })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GetMyGroupInvitationsResponse
                {
                    Message = ex.Message,
                    Invitations = null,
                    TotalCount = 0
                });
            }
        }

        [Authorize]
        [HttpPost("{groupId}/ban-member")]
        public async Task<IActionResult> BanMember(Guid groupId, [FromBody] BanMemberRequest request)
        {
            try
            {
                var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var (status, result) = await _groupService.BanMemberAsync(groupId, request.TargetUserId, currentUserId);

                return status switch
                {
                    BanMemberEnum.Success => Ok(new BanMemberResponse { Message = status.GetMessage(), Success = true }),
                    BanMemberEnum.GroupNotFound => NotFound(new BanMemberResponse { Message = status.GetMessage(), Success = false }),
                    BanMemberEnum.UserNotFound => NotFound(new BanMemberResponse { Message = status.GetMessage(), Success = false }),
                    BanMemberEnum.TargetUserNotMember => BadRequest(new BanMemberResponse { Message = status.GetMessage(), Success = false }),
                    BanMemberEnum.Unauthorized => StatusCode(403, new BanMemberResponse { Message = status.GetMessage(), Success = false }),
                    BanMemberEnum.CannotBanSelf => BadRequest(new BanMemberResponse { Message = status.GetMessage(), Success = false }),
                    BanMemberEnum.CannotBanSuperAdmin => BadRequest(new BanMemberResponse { Message = status.GetMessage(), Success = false }),
                    BanMemberEnum.CannotBanAdmin => BadRequest(new BanMemberResponse { Message = status.GetMessage(), Success = false }),
                    BanMemberEnum.AdminCannotBanAdmin => StatusCode(403, new BanMemberResponse { Message = status.GetMessage(), Success = false }),
                    BanMemberEnum.AlreadyBanned => BadRequest(new BanMemberResponse { Message = status.GetMessage(), Success = false }),
                    BanMemberEnum.Failed => StatusCode(500, new BanMemberResponse { Message = status.GetMessage(), Success = false }),
                    _ => StatusCode(500, new BanMemberResponse { Message = "Unknown error occurred", Success = false })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BanMemberResponse { Message = ex.Message, Success = false });
            }
        }

        [Authorize]
        [HttpPost("{groupId}/unban-member")]
        public async Task<IActionResult> UnbanMember(Guid groupId, [FromBody] UnbanMemberRequest request)
        {
            try
            {
                var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var (status, result) = await _groupService.UnbanMemberAsync(groupId, request.TargetUserId, currentUserId);

                return status switch
                {
                    UnbanMemberEnum.Success => Ok(new UnbanMemberResponse { Message = status.GetMessage(), Success = true }),
                    UnbanMemberEnum.GroupNotFound => NotFound(new UnbanMemberResponse { Message = status.GetMessage(), Success = false }),
                    UnbanMemberEnum.UserNotFound => NotFound(new UnbanMemberResponse { Message = status.GetMessage(), Success = false }),
                    UnbanMemberEnum.UserNotBanned => BadRequest(new UnbanMemberResponse { Message = status.GetMessage(), Success = false }),
                    UnbanMemberEnum.Unauthorized => StatusCode(403, new UnbanMemberResponse { Message = status.GetMessage(), Success = false }),
                    UnbanMemberEnum.Failed => StatusCode(500, new UnbanMemberResponse { Message = status.GetMessage(), Success = false }),
                    _ => StatusCode(500, new UnbanMemberResponse { Message = "Unknown error occurred", Success = false })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new UnbanMemberResponse { Message = ex.Message, Success = false });
            }
        }

        [Authorize]
        [HttpGet("{groupId}/banned-members")]
        public async Task<IActionResult> GetBannedMembers(Guid groupId, [FromQuery] int skip = 0, [FromQuery] int take = 10)
        {
            try
            {
                var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var (status, bannedMembers) = await _groupService.GetBannedMembersAsync(groupId, currentUserId, skip, take);

                return status switch
                {
                    GetBannedMembersEnum.Success => Ok(new GetBannedMembersResponse { Message = status.GetMessage(), BannedMembers = bannedMembers, TotalCount = bannedMembers?.Count ?? 0 }),
                    GetBannedMembersEnum.GroupNotFound => NotFound(new GetBannedMembersResponse { Message = status.GetMessage(), BannedMembers = null, TotalCount = 0 }),
                    GetBannedMembersEnum.Unauthorized => StatusCode(403, new GetBannedMembersResponse { Message = status.GetMessage(), BannedMembers = null, TotalCount = 0 }),
                    GetBannedMembersEnum.NoBannedMembers => Ok(new GetBannedMembersResponse { Message = status.GetMessage(), BannedMembers = new List<GroupUserDto>(), TotalCount = 0 }),
                    GetBannedMembersEnum.Failed => StatusCode(500, new GetBannedMembersResponse { Message = status.GetMessage(), BannedMembers = null, TotalCount = 0 }),
                    _ => StatusCode(500, new GetBannedMembersResponse { Message = "Unknown error occurred", BannedMembers = null, TotalCount = 0 })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GetBannedMembersResponse
                {
                    Message = ex.Message,
                    BannedMembers = null,
                    TotalCount = 0
                });
            }
        }
    }
}