using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using Service.Interfaces;
using System.Net;

namespace SWP391BackEnd.Controllers
{
    [ApiController]
    [Route("api/invoice-payments")]
    [ApiExplorerSettings(GroupName = "Invoice Payments")]
    public class MemberInvoicePaymentsController : ControllerBase
    {
        private readonly IMemberInvoicePaymentService _svc;

        public MemberInvoicePaymentsController(IMemberInvoicePaymentService svc)
        {
            _svc = svc;
        }

        [HttpPost("{invoiceId}")]
        [Authorize(Roles = "Co-owner")]
        public async Task<IActionResult> Create(Guid invoiceId, [FromQuery] string returnUrl, [FromQuery] string cancelUrl)
        {
            var userId = Guid.Parse(User.FindFirst("id")!.Value);
            var result = await _svc.CreatePaymentForInvoiceAsync(userId, invoiceId, returnUrl, cancelUrl);
            return StatusCode((int)HttpStatusCode.Created, result);
        }

        [HttpPost("webhook")]
        [AllowAnonymous] 
        public async Task<IActionResult> Webhook([FromBody] WebhookType webhook)
        {
            await _svc.HandleWebhookAsync(webhook);
            return Ok(new { message = "ok" });
        }

        [HttpGet("status/{orderCode}")]
        [Authorize]
        public async Task<IActionResult> Status(string orderCode)
        {
            var result = await _svc.GetPaymentStatusAsync(orderCode);
            if (result == null) return NotFound(new { message = "Không tìm thấy đơn thanh toán" });
            return Ok(result);
        }

        [HttpGet("history/my")]
        [Authorize(Roles = "Co-owner")]
        public async Task<IActionResult> MyHistory()
        {
            var userId = Guid.Parse(User.FindFirst("id")!.Value);
            var result = await _svc.GetUserPaymentHistoryAsync(userId);
            return Ok(result);
        }

        [HttpGet("invoice/{invoiceId}")]
        [Authorize(Roles = "Co-owner,Admin,Staff")]
        public async Task<IActionResult> ByInvoice(Guid invoiceId)
        {
            var result = await _svc.GetInvoicePaymentsAsync(invoiceId);
            return Ok(result);
        }
    }
}
