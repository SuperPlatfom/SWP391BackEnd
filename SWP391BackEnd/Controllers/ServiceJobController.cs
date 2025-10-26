using BusinessObject.DTOs.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using SWP391BackEnd.Helpers;
using System.Net;

namespace SWP391BackEnd.Controllers
{

    [ApiController]
    [Route("api/service-jobs")]
    [ApiExplorerSettings(GroupName = "Service Job Management")]
    public class ServiceJobController : ControllerBase
    {
        private readonly IServiceJobService _service;

        public ServiceJobController(IServiceJobService service)
        {
            _service = service;
        }


        [HttpGet]
        [Authorize(Roles = "Admin,Technician")]
        public async Task<IActionResult> GetAll([FromQuery] Guid? technicianId = null)
        {
            try
            {
                var list = await _service.GetAllAsync(technicianId);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK,
                    "Lấy danh sách công việc thành công", list);
            }
            catch (Exception ex)
            {
                return CustomErrorHandler.SimpleError(ex.Message, 500);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Technician")]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            try
            {
                var job = await _service.GetByIdAsync(id);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK,
                    "Lấy chi tiết công việc thành công", job);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return CustomErrorHandler.SimpleError(ex.Message, 500);
            }
        }

        [HttpPut("{id}/update-status")]
        [Authorize(Roles = "Admin,Technician")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateServiceJobStatusRequest req)
        {
            try
            {
                var technicianId = Guid.Parse(User.FindFirst("id")?.Value ?? throw new UnauthorizedAccessException());
                await _service.UpdateStatusAsync(id, req, technicianId);

                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK,
                    "Cập nhật trạng thái công việc thành công", null);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return CustomErrorHandler.SimpleError(ex.Message, 400);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return CustomErrorHandler.SimpleError(ex.Message, 500);
            }
        }

        [HttpPut("{id}/report")]
        [Authorize(Roles = "Admin,Technician")]
        public async Task<IActionResult> UpdateReport(Guid id, [FromBody] UpdateServiceJobReportRequest req)
        {
            try
            {
                var technicianId = Guid.Parse(User.FindFirst("id")?.Value ?? throw new UnauthorizedAccessException());
                await _service.UpdateReportAsync(id, req, technicianId);

                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK,
                    "Cập nhật báo cáo công việc thành công", null);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return CustomErrorHandler.SimpleError(ex.Message, 500);
            }
        }
    }
}
