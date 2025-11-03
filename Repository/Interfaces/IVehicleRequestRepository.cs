

using BusinessObject.Models;
using System.Linq.Expressions;

namespace Repository.Interfaces
{
    public interface IVehicleRequestRepository 
    {
        Task<VehicleRequest> AddAsync(VehicleRequest entity);
         Task<IEnumerable<VehicleRequest>> GetAllRequestsAsync();
        Task<IEnumerable<VehicleRequest>> GetPendingAsync();
        Task<VehicleRequest?> GetByIdWithDetailAsync(Guid id);
        Task<IEnumerable<VehicleRequest>> GetRequestsByUserAsync(Guid userId);
        Task<VehicleRequest> UpdateAsync(VehicleRequest entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Expression<Func<VehicleRequest, bool>> predicate);
    }
}
