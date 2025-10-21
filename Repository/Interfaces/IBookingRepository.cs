

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
        Task<bool> ExistsAsync(Guid id);
    }
}
