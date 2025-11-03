
using BusinessObject.Models;
using DataAccessLayer.DataContext;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System.Linq.Expressions;

namespace Repository
{
    public class VehicleRequestRepository : IVehicleRequestRepository
    {
        private readonly AppDbContext _context;

        public VehicleRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<VehicleRequest> AddAsync(VehicleRequest request)
        {
            await _context.VehicleRequests.AddAsync(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<VehicleRequest?> GetByIdWithDetailAsync(Guid id)
        {
            return await _context.VehicleRequests
                .Include(vr => vr.Requester)
                .Include(vr => vr.Vehicle)
                .FirstOrDefaultAsync(vr => vr.Id == id);
        }

        public async Task<IEnumerable<VehicleRequest>> GetAllRequestsAsync()
        {
            return await _context.VehicleRequests
                .Include(vr => vr.Requester)
                .Include(vr => vr.Vehicle)
                .OrderByDescending(vr => vr.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<VehicleRequest>> GetPendingAsync()
        {
            return await _context.VehicleRequests
                .Where(r => r.Status == "PENDING")
                .OrderBy(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<VehicleRequest>> GetRequestsByUserAsync(Guid userId)
        {
            return await _context.VehicleRequests
                .Include(vr => vr.Vehicle)
                .Where(vr => vr.CreatedBy == userId)
                .OrderByDescending(vr => vr.CreatedAt)
                .ToListAsync();
        }


        public async Task<VehicleRequest> UpdateAsync(VehicleRequest request)
        {
            _context.VehicleRequests.Update(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task DeleteAsync(Guid id)
        {
            var request = await _context.VehicleRequests.FindAsync(id);
            if (request != null)
            {
                _context.VehicleRequests.Remove(request);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> ExistsAsync(Expression<Func<VehicleRequest, bool>> predicate)
        {
            return await _context.VehicleRequests.AnyAsync(predicate);
        }
    }
}

