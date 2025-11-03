

using BusinessObject.Models;

namespace Repository.Interfaces
{
    public interface IBookingRepository 
    {
        Task<IEnumerable<Booking>> GetAllAsync();
        Task<Booking?> GetByIdAsync(Guid id);
        Task AddAsync(Booking booking);
        Task UpdateAsync(Booking booking);
        Task DeleteAsync(Booking booking);
        Task SaveChangesAsync();
        Task<bool> ExistsAsync(Guid id);
        Task<List<Booking>> GetBookingsByVehicleAsync(Guid vehicleId);
        Task<List<Booking>> GetUserBookingsByVehicleAsync(Guid userId, Guid vehicleId);
        Task<IEnumerable<Booking>> GetBookingsByVehicleInGroupAsync(Guid groupId, Guid vehicleId);
    }
}
