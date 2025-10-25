using BusinessObject.DTOs.ResponseModels;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class MemberInvoiceService : IMemberInvoiceService
    {
        private readonly IMemberInvoiceRepository _repo;

        public MemberInvoiceService(IMemberInvoiceRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<MemberInvoiceDto>> GetMyInvoicesAsync(Guid currentUserId)
        {
            var list = await _repo.GetByUserAsync(currentUserId);
            return list.Select(i => new MemberInvoiceDto
            {
                Id = i.Id,
                ExpenseId = i.ExpenseId,
                Title = i.Title,
                TotalAmount = i.TotalAmount,
                AmountPaid = i.AmountPaid,
                Status = i.Status,
                CreatedAt = i.CreatedAt
            }).ToList();
        }

        public async Task<MemberInvoiceDto> GetDetailAsync(Guid invoiceId)
        {
            var entity = await _repo.GetByIdAsync(invoiceId)
                ?? throw new KeyNotFoundException("Không tìm thấy hóa đơn");
            return new MemberInvoiceDto
            {
                Id = entity.Id,
                ExpenseId = entity.ExpenseId,
                Title = entity.Title,
                TotalAmount = entity.TotalAmount,
                AmountPaid = entity.AmountPaid,
                Status = entity.Status,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}
