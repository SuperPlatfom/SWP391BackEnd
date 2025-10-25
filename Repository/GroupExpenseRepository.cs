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

    public class GroupExpenseRepository : IGroupExpenseRepository
    {
        private readonly AppDbContext _db;
        public GroupExpenseRepository(AppDbContext db) => _db = db;

        public async Task<GroupExpense?> GetByIdAsync(Guid id)
        {
            return await _db.GroupExpenses
                .Include(e => e.MemberInvoices)
                .Include(e => e.Group)
                .Include(e => e.ServiceRequest)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<GroupExpense>> GetByGroupAsync(Guid groupId)
        {
            return await _db.GroupExpenses
                .Where(e => e.GroupId == groupId)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(GroupExpense entity)
        {
            await _db.GroupExpenses.AddAsync(entity);
        }

        public async Task UpdateAsync(GroupExpense entity)
        {
            _db.GroupExpenses.Update(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
        public async Task<GroupExpense?> GetByRequestIdAsync(Guid requestId)
        {
            return await _db.GroupExpenses
                .Include(e => e.ServiceRequest)
                .FirstOrDefaultAsync(e => e.ServiceRequestId == requestId);
        }

    }
}
