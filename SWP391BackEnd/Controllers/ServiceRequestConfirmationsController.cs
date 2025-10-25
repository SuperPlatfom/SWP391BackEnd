using BusinessObject.DTOs.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using SWP391BackEnd.Helpers;
using System.Net;

namespace SWP391BackEnd.Controllers
{
    [ApiController]
    [Route("api/service-request-confirmations")]
    [ApiExplorerSettings(GroupName = "Service Request Confirmation")]
    public class ServiceRequestConfirmationsController : Controller
    {
        private readonly IServiceRequestConfirmationService _service;

        public ServiceRequestConfirmationsController(IServiceRequestConfirmationService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Confirm([FromBody] ConfirmServiceRequestRequest req)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst("id")!.Value);

                var result = await _service.ConfirmAsync(userId, req.RequestId, req.Confirm, req.Reason);

                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK,
                    "Gửi biểu quyết thành công", result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return CustomErrorHandler.SimpleError(ex.Message, 400);
            }
        }

        [HttpGet("{requestId}")]
        [Authorize]
        public async Task<IActionResult> GetConfirmations(Guid requestId)
        {
            var list = await _service.GetByRequestIdAsync(requestId);
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK,
                "Lấy danh sách biểu quyết thành công", list);
        }
    }

}
