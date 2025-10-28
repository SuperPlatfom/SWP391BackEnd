using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IMemberInvoiceRepository
    {
        Task<IEnumerable<MemberInvoice>> GetByUserAsync(Guid userId);
        Task<MemberInvoice?> GetByIdAsync(Guid id);
        Task<IEnumerable<MemberInvoice>> GetByExpenseIdAsync(Guid expenseId);
        Task AddAsync(MemberInvoice entity);
        Task UpdateAsync(MemberInvoice entity);
        Task<MemberInvoice?> GetByExpenseAndUserAsync(Guid expenseId, Guid userId);
        Task SaveChangesAsync();
    }
}
