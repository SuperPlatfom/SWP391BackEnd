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
        private readonly ICoOwnershipGroupRepository _groupRepo;
        private readonly IGroupMemberRepository _groupMemberRepo;
        private readonly IVehicleRepository _vehicleRepo;
        private readonly IServiceCenterRepository _serviceCenterRepo;
        private readonly IEContractRepository _contractRepo;
        private readonly IAccountRepository _accountRepo;

        public ServiceRequestService(IServiceRequestRepository repo, ICoOwnershipGroupRepository groupRepo, IGroupMemberRepository groupMemberRepo, IVehicleRepository vehicleRepo, IServiceCenterRepository serviceCenterRepo, IEContractRepository contractRepo, IAccountRepository accountRepo)
        {
            _repo = repo;
            _groupRepo = groupRepo;
            _groupMemberRepo = groupMemberRepo;
            _vehicleRepo = vehicleRepo;
            _serviceCenterRepo = serviceCenterRepo;
            _contractRepo = contractRepo;
            _accountRepo = accountRepo;
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
                InspectionScheduledAt = x.InspectionScheduledAt,
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

            var group = await _groupRepo.GetByIdAsync(req.GroupId)
                ?? throw new KeyNotFoundException("Nhóm đồng sở hữu không tồn tại.");

            if (!await _groupMemberRepo.IsMemberAsync(req.GroupId, currentUserId))
                throw new UnauthorizedAccessException("Bạn không thuộc nhóm này.");


            var vehicle = await _vehicleRepo.GetByIdAsync(req.VehicleId)
                ?? throw new KeyNotFoundException("Phương tiện không tồn tại.");

            if (vehicle.Status != "ACTIVE")
                throw new InvalidOperationException("Phương tiện hiện không hoạt động, không thể yêu cầu dịch vụ.");

            if (!await _vehicleRepo.IsActiveInGroupAsync(req.VehicleId, req.GroupId))
                throw new InvalidOperationException("Xe không thuộc nhóm hoặc không ở trạng thái hoạt động.");

            var center = await _serviceCenterRepo.GetByIdAsync(req.ServiceCenterId)
                ?? throw new KeyNotFoundException("Trung tâm dịch vụ không tồn tại.");

            if (!center.IsActive)
                throw new InvalidOperationException("Trung tâm dịch vụ hiện đang ngừng hoạt động.");

            var contract = await _contractRepo.GetLatestApprovedByGroupAndVehicleAsync(req.GroupId, req.VehicleId);
            if (contract == null)
                throw new InvalidOperationException("Xe này chưa có hợp đồng đồng sở hữu hợp lệ. Vui lòng tạo hợp đồng trước khi yêu cầu dịch vụ.");


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
                InspectionScheduledAt = x.InspectionScheduledAt,
                CreatedAt = DateTimeHelper.ToVietnamTime(x.CreatedAt)
            }).ToList();
        }

        public async Task<IEnumerable<ServiceRequestDto>> GetByGroupAsync(Guid groupId, Guid currentUserId)
        {
            if (!await _groupMemberRepo.IsMemberAsync(groupId, currentUserId))
                throw new UnauthorizedAccessException("Bạn không thuộc nhóm này.");

            var list = await _repo.GetByGroupIdAsync(groupId);

            return list.Select(x => new ServiceRequestDto
            {
                Id = x.Id,
                Title = x.Title,
                Type = x.Type.ToString(),
                Status = x.Status,
                CostEstimate = x.CostEstimate,
                InspectionScheduledAt = x.InspectionScheduledAt,
                CreatedAt = DateTimeHelper.ToVietnamTime(x.CreatedAt)
            }).ToList();
        }

        public async Task<IEnumerable<ServiceRequestDto>> GetMyRequestsAsync(Guid currentUserId)
        {
            var list = await _repo.GetAllAsync();
            list = list.Where(x => x.CreatedBy == currentUserId);

            return list.Select(x => new ServiceRequestDto
            {
                Id = x.Id,
                Title = x.Title,
                Type = x.Type.ToString(),
                Status = x.Status,
                CostEstimate = x.CostEstimate,
                InspectionScheduledAt = x.InspectionScheduledAt,
                CreatedAt = DateTimeHelper.ToVietnamTime(x.CreatedAt)
            }).ToList();
        }

        public async Task<IEnumerable<ServiceRequestDto>> GetAssignedRequestsByUserAsync(Guid currentUserId)
        {
            var account = await _accountRepo.GetByIdAsync(currentUserId)
                ?? throw new KeyNotFoundException("Không tìm thấy người dùng.");

            var role = account.Role?.Name?.ToLower() ?? "";

            IEnumerable<ServiceRequest> list;

            if (role == "technician")
            {
                list = await _repo.GetByTechnicianAsync(currentUserId);
            }
            else if (role == "admin" || role == "staff")
            {
                list = await _repo.GetByStatusesAsync(new[]
                {
            "PENDING_QUOTE", "VOTING", "APPROVED", "IN_PROGRESS", "COMPLETED"
        });
            }
            else
            {
                throw new UnauthorizedAccessException("Bạn không có quyền xem danh sách này.");
            }

            return list.Select(x => new ServiceRequestDto
            {
                Id = x.Id,
                Title = x.Title,
                Type = x.Type.ToString(),
                Status = x.Status,
                CostEstimate = x.CostEstimate,
                TechnicianName = x.Technician?.FullName,
                InspectionScheduledAt = x.InspectionScheduledAt,
                CreatedAt = DateTimeHelper.ToVietnamTime(x.CreatedAt)
            }).ToList();
        }



    }
}
