using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using SWP391BackEnd.Helpers;
using System.Net;

namespace SWP391BackEnd.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [ApiExplorerSettings(GroupName = "Notification Management")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notiService;

        public NotificationController(INotificationService notiService)
        {
            _notiService = notiService;
        }

        private Guid CurrentUserId =>
            Guid.Parse(User.FindFirst("id")!.Value);

        [HttpGet]
        public async Task<IActionResult> GetMyNotifications()
        {
            var result = await _notiService.GetMyAsync(CurrentUserId);

            return CustomSuccessHandler.ResponseBuilder(
                HttpStatusCode.OK,
                "Lấy danh sách thông báo thành công",
                result);
        }


        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            await _notiService.MarkAsReadAsync(id);

            return CustomSuccessHandler.ResponseBuilder(
                HttpStatusCode.OK,
                "Đánh dấu thông báo đã đọc thành công",
                null);
        }

        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            await _notiService.MarkAllAsReadAsync(CurrentUserId);

            return CustomSuccessHandler.ResponseBuilder(
                HttpStatusCode.OK,
                "Đã đánh dấu tất cả thông báo là đã đọc",
                null);
        }
    }
}
