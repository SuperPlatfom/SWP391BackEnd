using BusinessObject.DTOs.RequestModels;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface ITripEventRepository
    {
        Task AddAsync(TripEvent tripEvent);
        Task UpdateAsync(TripEvent tripEvent);
        Task DeleteAsync(TripEvent tripEvent);
        Task<IEnumerable<TripEvent>> GetAllAsync();
        Task<IEnumerable<TripEvent>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<TripEvent>> GetDamageReportsByVehicleIdAsync(Guid vehicleId);


    }
}
