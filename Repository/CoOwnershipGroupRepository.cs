using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using DataAccessLayer.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Repository.Interfaces;


namespace Repository
{
    public class CoOwnershipGroupRepository : ICoOwnershipGroupRepository
    {
        private readonly AppDbContext _context;

        public CoOwnershipGroupRepository(AppDbContext context)
        {
            _context = context;
        }

        // --- Account ---
        public async Task<bool> AccountExistsAsync(Guid accountId)
        {
            return await _context.Accounts.AnyAsync(a => a.Id == accountId);
        }

        // --- CoOwnershipGroup ---
        public async Task AddGroupAsync(CoOwnershipGroup group)
        {
            _context.CoOwnershipGroups.Add(group);
            await _context.SaveChangesAsync();
        }

        public async Task<CoOwnershipGroup?> GetGroupByIdAsync(Guid groupId)
        {
            return await _context.CoOwnershipGroups
                .Include(g => g.Members)
                .Include(g => g.Vehicles)
                .FirstOrDefaultAsync(g => g.Id == groupId);
        }

        public async Task<List<CoOwnershipGroup>> GetGroupsByUserAsync(Guid userId)
        {
            return await _context.GroupMembers
                .Where(gm => gm.UserId == userId && gm.InviteStatus == "ACCEPTED")
                .Include(gm => gm.Group)
                    .ThenInclude(g => g.Vehicles)
                .Select(gm => gm.Group)
                .ToListAsync();
        }

        public async Task<List<CoOwnershipGroup>> GetAllGroupsAsync()
        {
            return await _context.CoOwnershipGroups
    .Include(g => g.Vehicles)
    .Include(g => g.Members)
        .ThenInclude(m => m.UserAccount)
    .ToListAsync();
        }

        public async Task UpdateGroupAsync(CoOwnershipGroup group)
        {
            _context.CoOwnershipGroups.Update(group);
            await _context.SaveChangesAsync();
        }
        // --- Vehicle ---
        public async Task<Vehicle?> GetVehicleByIdAsync(Guid vehicleId)
        {
            return await _context.Vehicles.FindAsync(vehicleId);
        }

        public async Task UpdateVehicleAsync(Vehicle vehicle)
        {
            _context.Vehicles.Update(vehicle);
            await _context.SaveChangesAsync();
        }


        public async Task<CoOwnershipGroup?> GetByIdAsync(Guid id)
        {
            return await _context.CoOwnershipGroups
                .Include(g => g.Members)
                 .ThenInclude(m => m.UserAccount)
                .Include(g => g.Vehicles)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<Account?> GetAccountByIdAsync(Guid accountId)
        {
            return await _context.Accounts.FindAsync(accountId);
        }

        // --- Transaction support ---
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        
    }
}
