using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using Repository.Interfaces;
using Service.Helpers;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly IServiceRequestRepository _repo;

        public ServiceRequestService(IServiceRequestRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<ServiceRequestDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();

            return list.Select(x => new ServiceRequestDto
            {
                Id = x.Id,
                Title = x.Title,
                Type = x.Type.ToString(),
                Status = x.Status,
                CostEstimate = x.CostEstimate,
                CreatedAt = DateTimeHelper.ToVietnamTime(x.CreatedAt)
            }).ToList();
        }

        public async Task<ServiceRequestDetailDto> GetDetailAsync(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Không tìm thấy yêu cầu dịch vụ");

            return new ServiceRequestDetailDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Type = entity.Type.ToString(),
                Description = entity.Description,
                Status = entity.Status,
                CostEstimate = entity.CostEstimate,
                InspectionScheduledAt = entity.InspectionScheduledAt,
                InspectionNotes = entity.InspectionNotes,
                ServiceCenterName = entity.ServiceCenter?.Name,
                TechnicianName = entity.Technician?.FullName,
                CreatedAt = DateTimeHelper.ToVietnamTime(entity.CreatedAt),
                UpdatedAt = DateTimeHelper.ToVietnamTime(entity.UpdatedAt)
            };
        }

        public async Task<ServiceRequestDetailDto> CreateAsync(CreateServiceRequestRequest req, Guid currentUserId)
        {
            var entity = new ServiceRequest
            {
                Id = Guid.NewGuid(),
                GroupId = req.GroupId,
                VehicleId = req.VehicleId,
                ServiceCenterId = req.ServiceCenterId,
                Type = req.Type,
                Title = req.Title,
                Description = req.Description,
                CreatedBy = currentUserId,
                Status = "DRAFT",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            return await GetDetailAsync(entity.Id);
        }

        public async Task<ServiceRequestDetailDto> UpdateInspectionScheduleAsync(Guid id, UpdateInspectionScheduleRequest req, Guid technicianId)
        {
            var entity = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Không tìm thấy yêu cầu dịch vụ");

            entity.InspectionScheduledAt = req.InspectionScheduledAt;
            entity.InspectionNotes = req.InspectionNotes;
            entity.TechnicianId = technicianId;
            entity.Status = "PENDING_QUOTE";
            entity.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(entity);
            await _repo.SaveChangesAsync();

            return await GetDetailAsync(id);
        }

        public async Task<ServiceRequestDetailDto> ProvideCostEstimateAsync(Guid id, ProvideCostEstimateRequest req, Guid technicianId)
        {
            var entity = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Không tìm thấy yêu cầu dịch vụ");

            if (entity.TechnicianId != technicianId)
                throw new UnauthorizedAccessException("Bạn không có quyền gửi báo giá cho yêu cầu này");

            entity.CostEstimate = req.CostEstimate;
            entity.Status = "VOTING";
            entity.UpdatedAt = DateTime.UtcNow;
            entity.InspectionNotes = req.Notes;

            await _repo.UpdateAsync(entity);
            await _repo.SaveChangesAsync();

            return await GetDetailAsync(id);
        }
        public async Task<IEnumerable<ServiceRequestDto>> GetMyGroupRequestsAsync(Guid currentUserId)
        {
            var list = await _repo.GetByGroupMembersAsync(currentUserId);

            return list.Select(x => new ServiceRequestDto
            {
                Id = x.Id,
                Title = x.Title,
                Type = x.Type.ToString(),
                Status = x.Status,
                CostEstimate = x.CostEstimate,
                CreatedAt = DateTimeHelper.ToVietnamTime(x.CreatedAt)
            }).ToList();
        }

    }
}
