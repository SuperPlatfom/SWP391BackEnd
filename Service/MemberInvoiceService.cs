using BusinessObject.DTOs.ResponseModels;
using Repository.Interfaces;
using Service.Helpers;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service
{
    public class MemberInvoiceService : IMemberInvoiceService
    {
        private readonly IMemberInvoiceRepository _invoiceRepo;
        private readonly IEContractMemberShareRepository _shareRepo;

        public MemberInvoiceService(
            IMemberInvoiceRepository invoiceRepo,
            IEContractMemberShareRepository shareRepo)
        {
            _invoiceRepo = invoiceRepo;
            _shareRepo = shareRepo;
        }

        public async Task<IEnumerable<MemberInvoiceDto>> GetMyInvoicesAsync(Guid currentUserId)
        {
            var invoices = await _invoiceRepo.GetByUserAsync(currentUserId);

            var result = new List<MemberInvoiceDto>();

            foreach (var inv in invoices)
            {
                var share = await _shareRepo.GetActiveShareAsync(inv.GroupId, inv.UserId);
                var sharePercent = share?.OwnershipRate ?? 0;


                result.Add(new MemberInvoiceDto
                {
                    Id = inv.Id,
                    ExpenseId = inv.ExpenseId,
                    Title = inv.Title,
                    TotalAmount = inv.TotalAmount,
                    AmountPaid = inv.AmountPaid,
                    Status = inv.Status,
                    CreatedAt = DateTimeHelper.ToVietnamTime(inv.CreatedAt),
                    OwnershipSharePercent = sharePercent,
                });
            }

            return result;
        }


        public async Task<MemberInvoiceDto> GetDetailAsync(Guid invoiceId)
        {
            var inv = await _invoiceRepo.GetByIdAsync(invoiceId)
                ?? throw new KeyNotFoundException("Không tìm thấy hóa đơn");

            var share = await _shareRepo.GetActiveShareAsync(inv.GroupId, inv.UserId);
            var sharePercent = share?.OwnershipRate ?? 0;

            return new MemberInvoiceDto
            {
                Id = inv.Id,
                ExpenseId = inv.ExpenseId,
                Title = inv.Title,
                TotalAmount = inv.TotalAmount,
                AmountPaid = inv.AmountPaid,
                Status = inv.Status,
                CreatedAt = DateTimeHelper.ToVietnamTime(inv.CreatedAt),
                OwnershipSharePercent = sharePercent,
            };
        }
    }
}
