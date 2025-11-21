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
    
        private readonly IStatisticService _statisticService;

        public StatisticController( IStatisticService statisticService)
        {
        
            _statisticService = statisticService;
        }


        [HttpGet("revenue")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetRevenueStatistics()
        {
            var result = await _statisticService.GetRevenueStatisticsAsync();
            return Ok(new
            {
                isSuccess = true,
                message = "Thống kê doanh thu thành công",
                data = result
            });
        }
        [HttpGet("Revenue-statistic-range/admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetRevenueStatistic([FromQuery] StatisticRequest s)
        {
            try
            {
                var result = await _statisticService.GetRevenueStatisticAsync(s.Start, s.End);
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


        [HttpGet("group-and-statistics")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetGroupAndVehicleStatistics()
        {
            var data = await _statisticService.GetGroupVehicleStatisticsAsync();
            return Ok(new
            {
                isSuccess = true,
                message = "Thống kê group và vehicle thành công",
                data = data
            });
        }

        [HttpGet("contract-statistics")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetContractStatistics()
        {
            var data = await _statisticService.GetContractStatisticsAsync();
            return Ok(new
            {
                isSuccess = true,
                message = "Thống kê contract thành công",
                data = data
            });
        }
    }
}

