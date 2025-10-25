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
    [Route("api/service-centers")]
    [ApiExplorerSettings(GroupName = "Service Center Management")]
    public class ServiceCentersController : Controller
    {
        private readonly IServiceCenterService _service;

        public ServiceCentersController(IServiceCenterService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK,
                "Lấy danh sách trung tâm dịch vụ thành công", list);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            try
            {
                var detail = await _service.GetByIdAsync(id);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK,
                    "Lấy chi tiết trung tâm thành công", detail);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Create([FromBody] CreateServiceCenterRequest req)
        {
            try
            {
                var result = await _service.CreateAsync(req);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.Created,
                    "Tạo trung tâm dịch vụ thành công", result);
            }
            catch (Exception ex)
            {
                return CustomErrorHandler.SimpleError(ex.Message, 400);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateServiceCenterRequest req)
        {
            try
            {
                var result = await _service.UpdateAsync(id, req);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK,
                    "Cập nhật trung tâm dịch vụ thành công", result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK,
                    "Xóa trung tâm dịch vụ thành công", null);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
