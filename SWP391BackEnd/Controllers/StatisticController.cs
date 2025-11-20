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

        public StatisticController(IServiceRequestService serviceRequestService)
        {
            _serviceRequestService = serviceRequestService;
        }
        [HttpPost("Revenue-statistic/admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetRevenueStatistic([FromBody] StatisticRequest s) 
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
        
    }
}
