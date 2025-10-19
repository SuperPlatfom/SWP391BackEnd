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

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateContract(Guid id, [FromBody] CreateContractRequest request)
        {
            try
            {
                var currentUserId = Guid.Parse(User.FindFirst("id")?.Value ?? throw new UnauthorizedAccessException());
                var result = await _contractService.UpdateAsync(id, request, currentUserId);

                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK,
                    "Cập nhật hợp đồng thành công",
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
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return CustomErrorHandler.SimpleError(ex.Message, 500);
            }
        }

        [HttpGet("my")]
        [Authorize]
        public async Task<IActionResult> GetMyContracts()
        {
            var currentUserId = Guid.Parse(User.FindFirst("id")!.Value);
            var list = await _contractService.GetMyContractsAsync(currentUserId);
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK,
                "Lấy danh sách hợp đồng mà bạn tham gia thành công", list);
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

        [HttpPost("{id}/send-otp")]
        [Authorize]
        public async Task<IActionResult> SendContractOtp(Guid id)
        {
            var userId = Guid.Parse(User.FindFirst("id")!.Value);
            await _contractService.SendOtpAsync(id, userId);
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK,
                "Đã gửi OTP ký hợp đồng thành công", null);
        }

        [HttpPost("{id}/sign")]
        [Authorize]
        public async Task<IActionResult> SignContract(Guid id, [FromBody] VerifyContractOtpRequest req)
        {
            var userId = Guid.Parse(User.FindFirst("id")!.Value);
            await _contractService.VerifyOtpAsync(id, userId, req.Otp);
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK,
                "Ký hợp đồng thành công", null);
        }

        [HttpPost("{id}/cancel")]
        [Authorize]
        public async Task<IActionResult> CancelContract(Guid id)
        {
            try
            {
                var currentUserId = Guid.Parse(User.FindFirst("id")!.Value);
                await _contractService.CancelContractAsync(id, currentUserId);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK,
                    "Hủy hợp đồng thành công", null);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return CustomErrorHandler.SimpleError(ex.Message, 400);
            }
            catch (Exception ex)
            {
                return CustomErrorHandler.SimpleError(ex.Message, 500);
            }
        }

        [HttpPost("{id}/review")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> Review(Guid id, [FromBody] ReviewContractRequest req)
        {
            try
            {
                var staffId = Guid.Parse(User.FindFirst("id")!.Value);
                await _contractService.ReviewContractAsync(id, staffId, req.Approve, req.Note);
                return CustomSuccessHandler.ResponseBuilder(
                    HttpStatusCode.OK,
                    req.Approve ? "Phê duyệt hợp đồng thành công" : "Từ chối hợp đồng thành công",
                    null
                );
            }
            catch (KeyNotFoundException ex)
            {
                return CustomErrorHandler.SimpleError(ex.Message, 404);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return CustomErrorHandler.SimpleError(ex.Message, 400);
            }
            catch (Exception ex)
            {
                return CustomErrorHandler.SimpleError(ex.Message, 500);
            }
        }

    }
}
