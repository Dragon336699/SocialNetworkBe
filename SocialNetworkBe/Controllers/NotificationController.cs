using Domain.Contracts.Requests;
using Domain.Entities;
using Domain.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SocialNetworkBe.Controllers
{
    [ApiController]
    [Route("api/v1/notification/")]
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;
        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        [Route("getNotis")]
        public async Task<IActionResult> GetNotifications([FromQuery] int skip, int take)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return Unauthorized();

                var notis = await _notificationService.GetNotis(Guid.Parse(userId), skip, take);
                if (notis == null) return Ok(new { message = "Get notification successful", data = new List<Notification>() });
                return Ok(new { message = "Get notification successful", data = notis });
            } catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("getUnreadNotis")]
        public async Task<IActionResult> GetUnreadNotifications()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return Unauthorized();

                var notis = await _notificationService.GetUnreadNotifications(Guid.Parse(userId));
                return Ok(new { message = "Get unread notification successful", data = notis });
            } catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("markNotiAsRead")]
        public async Task<IActionResult> MarkNotiAsRead([FromBody] MarkNotiAsReadRequest request)
        {
            try
            {
                await _notificationService.MarkNotificationAsRead(request.NotificationId);
                return Ok(new { message = "Mark notification as read successful"});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("markAllNotisAsRead")]
        public async Task<IActionResult> MarkAllNotisAsRead()
        {
            try
            {
                await _notificationService.MarkAllNotificationsAsRead();
                return Ok(new { message = "Mark all notifications as read successful" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
