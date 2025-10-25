using BusinessObject.Models;
using DataAccessLayer.DataContext;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;

namespace Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _db;
        public PaymentRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Payment entity)
        {
            await _db.Payments.AddAsync(entity);
        }

        public async Task UpdateAsync(Payment entity)
        {
            _db.Payments.Update(entity);
            await Task.CompletedTask;
        }

        public async Task<List<Payment>> GetByUserIdAsync(Guid userId)
        {
            return await _db.Payments
                .Include(p => p.PayOSTransaction)
                .Include(p => p.Invoice)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Payment?> GetByIdAsync(Guid id)
        {
            return await _db.Payments
                .Include(p => p.PayOSTransaction)
                .Include(p => p.Invoice)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Payment?> GetByOrderCodeAsync(string orderCode)
        {
            return await _db.Payments
                .Include(p => p.PayOSTransaction)
                .Include(p => p.Invoice)
                .FirstOrDefaultAsync(p => p.TransactionCode == orderCode);
        }

        public async Task<List<Payment>> GetByInvoiceIdAsync(Guid invoiceId)
        {
            return await _db.Payments
                .Include(p => p.PayOSTransaction)
                .Where(p => p.InvoiceId == invoiceId)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
