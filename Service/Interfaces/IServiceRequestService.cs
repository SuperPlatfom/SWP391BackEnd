using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IServiceRequestService
    {
        Task<IEnumerable<ServiceRequestDto>> GetAllAsync();
        Task<ServiceRequestDetailDto> GetDetailAsync(Guid id);
        Task<ServiceRequestDetailDto> CreateAsync(CreateServiceRequestRequest req, Guid currentUserId);
        Task<ServiceRequestDetailDto> UpdateInspectionScheduleAsync(Guid id, UpdateInspectionScheduleRequest req, Guid technicianId);
        Task<IEnumerable<ServiceRequestDto>> GetMyGroupRequestsAsync(Guid currentUserId);
        Task<ServiceRequestDetailDto> ProvideCostEstimateAsync(Guid id, ProvideCostEstimateRequest req, Guid technicianId);
        Task<IEnumerable<ServiceRequestDto>> GetByGroupAsync(Guid groupId, Guid currentUserId);
        Task<IEnumerable<ServiceRequestDto>> GetMyRequestsAsync(Guid currentUserId);
        Task<IEnumerable<ServiceRequestDto>> GetAssignedRequestsByUserAsync(Guid currentUserId);
        

    }
}
