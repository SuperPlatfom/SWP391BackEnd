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
    [Route("api/service-requests")]
    [ApiExplorerSettings(GroupName = "Service Request Management")]
    public class ServiceRequestsController : Controller
    {
        private readonly IServiceRequestService _service;

        public ServiceRequestsController(IServiceRequestService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK,
                "Lấy danh sách yêu cầu dịch vụ thành công", result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            try
            {
                var result = await _service.GetDetailAsync(id);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK,
                    "Lấy chi tiết yêu cầu thành công", result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("my-group")]
        [Authorize]
        public async Task<IActionResult> GetMyGroupServiceRequests()
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst("id")!.Value);
                var result = await _service.GetMyGroupRequestsAsync(userId);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK,
                    "Lấy danh sách yêu cầu dịch vụ của nhóm thành công", result);
            }
            catch (Exception ex)
            {
                return CustomErrorHandler.SimpleError(ex.Message, 400);
            }
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromForm] CreateServiceRequestRequest req)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst("id")!.Value);
                var result = await _service.CreateAsync(req, userId);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.Created,
                    "Tạo yêu cầu dịch vụ thành công", result);
            }
            catch (Exception ex)
            {
                return CustomErrorHandler.SimpleError(ex.Message, 400);
            }
        }

        [HttpPut("{id}/schedule")]
        [Authorize]
        public async Task<IActionResult> Schedule(Guid id, [FromBody] UpdateInspectionScheduleRequest req)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst("id")!.Value);
                var result = await _service.UpdateInspectionScheduleAsync(id, req, userId);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK,
                    "Cập nhật lịch hẹn kiểm tra thành công", result);
            }
            catch (Exception ex)
            {
                return CustomErrorHandler.SimpleError(ex.Message, 400);
            }
        }

        [HttpPut("{id}/estimate")]
        [Authorize]
        public async Task<IActionResult> ProvideEstimate(Guid id, [FromBody] ProvideCostEstimateRequest req)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst("id")!.Value);
                var result = await _service.ProvideCostEstimateAsync(id, req, userId);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK,
                    "Gửi báo giá thành công", result);
            }
            catch (Exception ex)
            {
                return CustomErrorHandler.SimpleError(ex.Message, 400);
            }
        }

    }
}
