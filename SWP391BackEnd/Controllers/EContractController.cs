using System.Net;
using BusinessObject.DTOs.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using SWP391BackEnd.Helpers;

namespace SWP391BackEnd.Controllers
{
    [ApiController]
    [Route("api/contracts")]
    [ApiExplorerSettings(GroupName = "E-Contract Management")]
    public class EContractController : Controller
    {
        private readonly IContractService _contractService;

        public EContractController(IContractService contractService)
        {
            _contractService = contractService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateContractRequest request)
        {
            try
            {
                var currentUserId = Guid.Parse(User.FindFirst("id")?.Value ?? throw new UnauthorizedAccessException());
                var result = await _contractService.CreateAsync(request, currentUserId);

                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.Created,
                    "Hợp đồng được tạo thành công",
                    result);
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

   
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] string? status, [FromQuery] Guid? groupId)
        {

            {
                var currentUserId = Guid.Parse(User.FindFirst("id")?.Value ?? throw new UnauthorizedAccessException());
                var list = await _contractService.GetAllAsync(currentUserId, status, groupId);

                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK,
                    "Lấy danh sách hợp đồng thành công",
                    list);
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            try
            {
                var currentUserId = Guid.Parse(User.FindFirst("id")?.Value ?? throw new UnauthorizedAccessException());
                var detail = await _contractService.GetDetailAsync(id, currentUserId);

                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK,
                    "Lấy chi tiết hợp đồng thành công",
                    detail);
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


        [HttpGet("{id}/preview")]
        [Authorize]
        public async Task<IActionResult> GetPreview(Guid id)
        {
            try
            {
                var currentUserId = Guid.Parse(User.FindFirst("id")?.Value ?? throw new UnauthorizedAccessException());
                var html = await _contractService.GetPreviewHtmlAsync(id, currentUserId);
                return Content(html, "text/html");
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
