using BusinessObject.DTOs.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IMemberInvoiceService
    {
        Task<IEnumerable<MemberInvoiceDto>> GetMyInvoicesAsync(Guid currentUserId);
        Task<MemberInvoiceDto> GetDetailAsync(Guid invoiceId);
    }
}
