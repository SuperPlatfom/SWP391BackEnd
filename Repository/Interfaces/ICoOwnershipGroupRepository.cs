using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace Repository.Interfaces
{
    public interface ICoOwnershipGroupRepository
    {
        // --- Account ---
        Task<bool> AccountExistsAsync(Guid accountId);

        // --- CoOwnershipGroup ---
        Task AddGroupAsync(CoOwnershipGroup group);
        Task<CoOwnershipGroup?> GetByIdAsync(Guid id);
        Task<CoOwnershipGroup?> GetGroupByIdAsync(Guid groupId);
        Task<List<CoOwnershipGroup>> GetGroupsByUserAsync(Guid userId);
        Task<List<CoOwnershipGroup>> GetAllGroupsAsync();
        Task UpdateGroupAsync(CoOwnershipGroup group);

        // --- Vehicle ---
        Task<Vehicle?> GetVehicleByIdAsync(Guid vehicleId);
        Task UpdateVehicleAsync(Vehicle vehicle);

        // --- Transaction support ---
        Task<IDbContextTransaction> BeginTransactionAsync();
        
    }
}
