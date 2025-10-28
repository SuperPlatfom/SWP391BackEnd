using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IPaymentRepository
    {
        Task AddAsync(Payment entity);
        Task UpdateAsync(Payment entity);
        Task<List<Payment>> GetByUserIdAsync(Guid userId);
        Task<Payment?> GetByIdAsync(Guid id);
        Task<Payment?> GetByOrderCodeAsync(string orderCode);
        Task<List<Payment>> GetByInvoiceIdAsync(Guid invoiceId);
        Task SaveChangesAsync();
    }
}
