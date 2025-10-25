using BusinessObject.Models;
using DataAccessLayer.DataContext;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;

namespace Repository
{
    public class PayosTransactionRepository : IPayosTransactionRepository
    {
        private readonly AppDbContext _db;
        public PayosTransactionRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(PayOSTransaction entity)
        {
            await _db.PayOSTransactions.AddAsync(entity);
        }

        public async Task UpdateAsync(PayOSTransaction entity)
        {
            _db.PayOSTransactions.Update(entity);
            await Task.CompletedTask;
        }

        public async Task<PayOSTransaction?> GetByPaymentIdAsync(Guid paymentId)
        {
            return await _db.PayOSTransactions
                .Include(t => t.Payment)
                .FirstOrDefaultAsync(t => t.PaymentId == paymentId);
        }

        public async Task<PayOSTransaction?> GetByOrderCodeAsync(string orderCode)
        {
            return await _db.PayOSTransactions
                .Include(t => t.Payment)
                .FirstOrDefaultAsync(t => t.OrderCode == orderCode);
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
