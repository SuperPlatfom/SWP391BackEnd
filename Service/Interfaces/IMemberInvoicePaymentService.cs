using BusinessObject.DTOs.ResponseModels;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IMemberInvoicePaymentService
    {
        Task<CreatePaymentResult> CreatePaymentForInvoiceAsync(
            Guid userId, Guid invoiceId, string returnUrl, string cancelUrl);

        Task HandleWebhookAsync(WebhookType webhookData);

        Task<PaymentStatusResponse?> GetPaymentStatusAsync(string orderCode);

        Task<IEnumerable<PaymentHistoryItem>> GetUserPaymentHistoryAsync(Guid userId);

        Task<IEnumerable<PaymentHistoryItem>> GetInvoicePaymentsAsync(Guid invoiceId);
    }
}
