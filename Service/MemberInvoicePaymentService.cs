using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using Microsoft.Extensions.Configuration;
using Net.payOS.Types;
using Net.payOS;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class MemberInvoicePaymentService : IMemberInvoicePaymentService
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly IPayosTransactionRepository _txRepo;
        private readonly IMemberInvoiceRepository _invoiceRepo;
        private readonly PayOS _payOS;

        public MemberInvoicePaymentService(
            IConfiguration config,
            IPaymentRepository paymentRepo,
            IPayosTransactionRepository txRepo,
            IMemberInvoiceRepository invoiceRepo)
        {
            _paymentRepo = paymentRepo;
            _txRepo = txRepo;
            _invoiceRepo = invoiceRepo;

            _payOS = new PayOS(
                config["Environment:PAYOS_CLIENT_ID"],
                config["Environment:PAYOS_API_KEY"],
                config["Environment:PAYOS_CHECKSUM_KEY"]
            );
        }

        public async Task<CreatePaymentResult> CreatePaymentForInvoiceAsync(
            Guid userId, Guid invoiceId, string returnUrl, string cancelUrl)
        {
            var invoice = await _invoiceRepo.GetByIdAsync(invoiceId)
                ?? throw new KeyNotFoundException("Invoice không tồn tại");

            var outstanding = Math.Round(invoice.TotalAmount - invoice.AmountPaid, 2);
            if (outstanding <= 0)
                throw new InvalidOperationException("Invoice đã được thanh toán đủ");

            long orderCode = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                InvoiceId = invoice.Id,
                Amount = outstanding,
                PaymentMethod = "PayOs",
                Status = "PENDING",
                TransactionCode = orderCode.ToString(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _paymentRepo.AddAsync(payment);

            var tx = new PayOSTransaction
            {
                Id = Guid.NewGuid(),
                PaymentId = payment.Id,
                OrderCode = orderCode.ToString(),
                ExpiredAt = DateTime.UtcNow.AddMinutes(15),
                Status = "INIT",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var item = new ItemData($"Invoice #{invoice.Id.ToString()[..8]}", 1, (int)Math.Round(outstanding));
            var shortDesc = $"Invoice {invoice.Id.ToString()[..6]}"; 

            var request = new PaymentData(
                orderCode, (int)Math.Round(outstanding),
                shortDesc,
                new List<ItemData> { item },
                cancelUrl, returnUrl);


            var link = await _payOS.createPaymentLink(request);
            tx.QrCodeUrl = link.qrCode;
            tx.DeeplinkUrl = link.checkoutUrl;
            await _txRepo.AddAsync(tx);

            return link;
        }

        public async Task HandleWebhookAsync(WebhookType webhookData)
        {

            var order = webhookData.data.orderCode.ToString();
            var payment = await _paymentRepo.GetByOrderCodeAsync(order);
            if (payment == null) return;
            if (webhookData.data.code == "00")
            {
                payment.Status = "PAID";
                payment.PaidAt = DateTime.UtcNow;
                payment.UpdatedAt = DateTime.UtcNow;


                if (payment.InvoiceId.HasValue)
                {
                    var invoice = await _invoiceRepo.GetByIdAsync(payment.InvoiceId.Value);
                    if (invoice != null)
                    {
                        invoice.AmountPaid = Math.Round(invoice.AmountPaid + payment.Amount, 2);
                        if (invoice.AmountPaid + 0.00001m >= invoice.TotalAmount) 
                            invoice.Status = "PAID";
                        else if (invoice.AmountPaid > 0)
                            invoice.Status = "PARTIAL";
                        invoice.UpdatedAt = DateTime.UtcNow;
                        await _invoiceRepo.UpdateAsync(invoice);
                    }
                }

                var tx = await _txRepo.GetByOrderCodeAsync(order);
                if (tx != null)
                {
                    tx.Status = "PAID";
                    tx.WebhookReceivedAt = DateTime.UtcNow;
                    tx.UpdatedAt = DateTime.UtcNow;
                    await _txRepo.UpdateAsync(tx);
                }

                await _paymentRepo.UpdateAsync(payment);
            }
            else if (webhookData.data.code == "07" || webhookData.data.code == "09") 
            {
                payment.Status = "CANCELED";
                payment.UpdatedAt = DateTime.UtcNow;
                await _paymentRepo.UpdateAsync(payment);

                var tx = await _txRepo.GetByOrderCodeAsync(order);
                if (tx != null)
                {
                    tx.Status = webhookData.data.code == "07" ? "CANCELED" : "EXPIRED";
                    tx.WebhookReceivedAt = DateTime.UtcNow;
                    tx.UpdatedAt = DateTime.UtcNow;
                    await _txRepo.UpdateAsync(tx);
                }
            }

        }

        public async Task<PaymentStatusResponse?> GetPaymentStatusAsync(string orderCode)
        {
            var payment = await _paymentRepo.GetByOrderCodeAsync(orderCode);
            if (payment == null) return null;

            return new PaymentStatusResponse
            {
                OrderCode = orderCode,
                Amount = payment.Amount,
                Status = payment.Status,
                PaidAt = payment.PaidAt?.ToString("yyyy-MM-dd HH:mm:ss"),
                Title = $"Invoice #{payment.InvoiceId?.ToString()[..8]}"
            };
        }

        public async Task<IEnumerable<PaymentHistoryItem>> GetUserPaymentHistoryAsync(Guid userId)
        {
            var list = await _paymentRepo.GetByUserIdAsync(userId);
            return list.Select(p => new PaymentHistoryItem
            {
                OrderCode = p.TransactionCode,
                Amount = p.Amount,
                Status = p.Status,
                PaidAt = p.PaidAt?.ToString("yyyy-MM-dd HH:mm:ss"),
                Method = p.PaymentMethod,
                Description = $"Invoice #{p.InvoiceId?.ToString()[..8]}"
            });
        }

        public async Task<IEnumerable<PaymentHistoryItem>> GetInvoicePaymentsAsync(Guid invoiceId)
        {
            var list = await _paymentRepo.GetByInvoiceIdAsync(invoiceId);
            return list.Select(p => new PaymentHistoryItem
            {
                OrderCode = p.TransactionCode,
                Amount = p.Amount,
                Status = p.Status,
                PaidAt = p.PaidAt?.ToString("yyyy-MM-dd HH:mm:ss"),
                Method = p.PaymentMethod,
                Description = $"Invoice #{invoiceId.ToString()[..8]}"
            });
        }
    }
}
