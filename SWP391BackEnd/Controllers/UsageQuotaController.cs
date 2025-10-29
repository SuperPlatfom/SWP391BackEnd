using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Interfaces;

namespace SWP391BackEnd.Controllers
{
    [ApiController]
    [Route("api/quota")]
    [Authorize]
    [ApiExplorerSettings(GroupName = "Quota")]
    public class UsageQuotaController : Controller
    {
        private readonly IUsageQuotaService _quotaService;

        public UsageQuotaController(IUsageQuotaService quotaService)
        {
            _quotaService = quotaService;
        }

        [HttpGet("check/{groupId}/{vehicleId}")]

        public async Task<IActionResult> CheckQuota(Guid groupId, Guid vehicleId)
        {
            var result = await _quotaService.GetRemainingQuotaAsync(groupId, vehicleId, User);

            if (!result.IsSuccess)
                return BadRequest(new { result.Message });

            return Ok(new
            {
                result.Message,
                result.Data
            });
        }
        /// <summary>
        /// Reset toàn bộ quota (HoursUsed = 0)
        /// </summary>
        [HttpPost("reset")]
        public async Task<IActionResult> ResetQuotaAsync()
        {
            try
            {
                await _quotaService.ResetWeeklyQuotaAsync();
                return Ok(new
                {
                    message = "Reset quota thành công.",
                    time = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Lỗi khi reset quota.",
                    error = ex.Message
                });
            }
        }
    }
}
