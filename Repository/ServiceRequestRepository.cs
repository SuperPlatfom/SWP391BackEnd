using BusinessObject.Models;
using DataAccessLayer.DataContext;
using Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class ServiceRequestRepository : IServiceRequestRepository
    {
        private readonly AppDbContext _db;

        public ServiceRequestRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<ServiceRequest>> GetAllAsync()
        {
            return await _db.ServiceRequests
                .Include(s => s.ServiceCenter)
                .Include(s => s.Vehicle)
                .Include(s => s.Technician)
                .Include(s => s.Group)
                .AsNoTracking()
                .ToListAsync();
        }


        public async Task<List<ServiceRequest>> GetCompletedOrdersInRangeAsync(DateTime start, DateTime end)
        {
            return await _db.ServiceRequests
                .Include(s => s.Group)
                .Include(s => s.ServiceCenter)
                .Include(x => x.Vehicle)
                .Include(x => x.Technician)
                .Where(x => x.Status == "COMPLETED"
                         && x.CompletedAt >= start
                         && x.CompletedAt <= end)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ServiceRequest?> GetByIdAsync(Guid id)
        {
            return await _db.ServiceRequests
                .Include(s => s.ServiceCenter)
                .Include(s => s.Vehicle)
                .Include(s => s.Technician)
                .Include(s => s.Group)
                .Include(s => s.CreatedByAccount)
                .Include(e => e.Vehicle.Contracts)
                .Include(x => x.Job)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task AddAsync(ServiceRequest entity)
        {
            await _db.ServiceRequests.AddAsync(entity);
        }

        public async Task UpdateAsync(ServiceRequest entity)
        {
            _db.ServiceRequests.Update(entity);
        }

        public async Task<IEnumerable<ServiceRequest>> GetByGroupMembersAsync(Guid currentUserId)
        {

            var groupIds = await _db.GroupMembers
                .Where(gm => gm.UserId == currentUserId)
                .Select(gm => gm.GroupId)
                .ToListAsync();


            var memberIds = await _db.GroupMembers
                .Where(gm => groupIds.Contains(gm.GroupId))
                .Select(gm => gm.UserId)
                .Distinct()
                .ToListAsync();


            return await _db.ServiceRequests
                .Include(s => s.ServiceCenter)
                .Include(s => s.Vehicle)
                .Include(s => s.Technician)
                .Include(s => s.Group)
                .Where(s => memberIds.Contains(s.CreatedBy))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ServiceRequest?> GetByExpenseIdAsync(Guid expenseId)
        {
            var expense = await _db.GroupExpenses
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == expenseId);

            if (expense == null || expense.ServiceRequestId == Guid.Empty)
                return null;

            return await _db.ServiceRequests
                .Include(sr => sr.Vehicle)
                .Include(sr => sr.Technician)
                .Include(sr => sr.Group)
                .Include(sr => sr.ServiceCenter)
                .FirstOrDefaultAsync(sr => sr.Id == expense.ServiceRequestId);
        }

        public async Task<IEnumerable<ServiceRequest>> GetByGroupIdAsync(Guid groupId)
        {
            return await _db.ServiceRequests
                .Include(s => s.ServiceCenter)
                .Include(s => s.Vehicle)
                .Include(s => s.Technician)
                .Include(s => s.Group)
                .Where(s => s.GroupId == groupId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<ServiceRequest>> GetByTechnicianAsync(Guid technicianId)
        {
            return await _db.ServiceRequests
                .Include(x => x.Technician)
                .Include(x => x.ServiceCenter)
                .Include(x => x.Vehicle)
                .Include(x => x.Group)
                .Include(x => x.CreatedByAccount)
                .Where(x => x.TechnicianId == technicianId)
                .OrderByDescending(r => r.UpdatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ServiceRequest>> GetByStatusesAsync(IEnumerable<string> statuses)
        {
            return await _db.ServiceRequests
                .Include(x => x.Technician)
                .Include(x => x.ServiceCenter)
                .Include(x => x.Vehicle)
                .Include(x => x.Group)
                .Where(x => statuses.Contains(x.Status))
                .AsNoTracking()
                .ToListAsync();
        }


        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
