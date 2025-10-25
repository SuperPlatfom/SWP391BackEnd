using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using SWP391BackEnd.Helpers;

namespace SWP391BackEnd.Controllers
{
    [ApiController]
    [Route("api/member-invoices")]
    [ApiExplorerSettings(GroupName = "Member Invoice Management")]
    public class MemberInvoicesController : Controller
    {
        private readonly IMemberInvoiceService _service;

        public MemberInvoicesController(IMemberInvoiceService service)
        {
            _service = service;
        }

        [HttpGet("my")]
        [Authorize]
        public async Task<IActionResult> GetMyInvoices()
        {
            var userId = Guid.Parse(User.FindFirst("id")!.Value);
            var result = await _service.GetMyInvoicesAsync(userId);
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK,
                "Lấy danh sách hóa đơn của bạn thành công", result);
        }

        [HttpGet("{invoiceId}")]
        [Authorize]
        public async Task<IActionResult> GetDetail(Guid invoiceId)
        {
            try
            {
                var result = await _service.GetDetailAsync(invoiceId);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK,
                    "Lấy chi tiết hóa đơn thành công", result);
            }
            catch (Exception ex)
            {
                return CustomErrorHandler.SimpleError(ex.Message, 400);
            }
        }
    }
}
