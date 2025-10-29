using BusinessObject.DTOs.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IServiceRequestConfirmationService
    {
        Task<ServiceRequestConfirmationDto> ConfirmAsync(Guid currentUserId, Guid requestId, bool confirm, string? reason);
        Task<IEnumerable<ServiceRequestConfirmationDto>> GetByRequestIdAsync(Guid requestId);
        Task<IEnumerable<ServiceRequestConfirmationDto>> GetByUserAsync(Guid currentUserId);
    }
}
