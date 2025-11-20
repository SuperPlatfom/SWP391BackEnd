using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace SWP391BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Statistic")]
    public class StatisticController : Controller
    {
       private readonly IServiceRequestService _serviceRequestService;
        private readonly IStatisticService _statisticService;

        public StatisticController(IServiceRequestService serviceRequestService, IStatisticService statisticService)
        {
            _serviceRequestService = serviceRequestService;
            _statisticService = statisticService;
        }
        [HttpGet("Revenue-statistic/admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetRevenueStatistic([FromQuery] StatisticRequest s) 
        {
            try
            {
                var result = await _serviceRequestService.GetRevenueStatisticAsync(s.Start,s.End);
                var (isSuccess, message, data) = result;

                return Ok(new
                {
                    isSuccess,
                    message,
                    data
                });
            
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("user-statistics")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserStatistics()
        {
            var data = await _statisticService.GetUserStatisticsAsync();
            return Ok(new
            {
                isSuccess = true,
                message = "Thống kê user thành công",
                data = data
            });
        }

    }
}
