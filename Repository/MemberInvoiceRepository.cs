using BusinessObject.Models;
using DataAccessLayer.DataContext;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class MemberInvoiceRepository : IMemberInvoiceRepository
    {
        private readonly AppDbContext _db;
        public MemberInvoiceRepository(AppDbContext db) => _db = db;

        public async Task<IEnumerable<MemberInvoice>> GetByUserAsync(Guid userId)
        {
            return await _db.MemberInvoices
                .Include(i => i.Expense)
                .Where(i => i.UserId == userId)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
        }

        public async Task<MemberInvoice?> GetByIdAsync(Guid id)
        {
            return await _db.MemberInvoices
                .Include(i => i.Expense)
                .FirstOrDefaultAsync(i => i.Id == id);
        }
        public async Task<IEnumerable<MemberInvoice>> GetByExpenseIdAsync(Guid expenseId)
        {
            return await _db.MemberInvoices
                .Include(i => i.User)
                .Where(i => i.ExpenseId == expenseId)
                .ToListAsync();
        }
        public async Task AddAsync(MemberInvoice entity)
        {
            await _db.MemberInvoices.AddAsync(entity);
        }

        public async Task UpdateAsync(MemberInvoice entity)
        {
            _db.MemberInvoices.Update(entity);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
        public async Task<MemberInvoice?> GetByExpenseAndUserAsync(Guid expenseId, Guid userId)
        {
            return await _db.MemberInvoices
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.ExpenseId == expenseId && i.UserId == userId);
        }

    }
}

