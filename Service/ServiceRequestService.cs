using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using MediatR;
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
        private readonly INotificationService _notificationService;

        public ServiceRequestService(IServiceRequestRepository repo, ICoOwnershipGroupRepository groupRepo, IGroupMemberRepository groupMemberRepo, IVehicleRepository vehicleRepo, IServiceCenterRepository serviceCenterRepo, IEContractRepository contractRepo, IAccountRepository accountRepo, INotificationService notificationService)
        {
            _repo = repo;
            _groupRepo = groupRepo;
            _groupMemberRepo = groupMemberRepo;
            _vehicleRepo = vehicleRepo;
            _serviceCenterRepo = serviceCenterRepo;
            _contractRepo = contractRepo;
            _accountRepo = accountRepo;
            _notificationService = notificationService;
        }

        public async Task<IEnumerable<ServiceRequestDto>> GetAllAsync()
        {
            var list = (await _repo.GetAllAsync())
            .OrderByDescending(x => x.CreatedAt);

            return list.Select(x => new ServiceRequestDto
            {
                Id = x.Id,
                Title = x.Title,
                Type = x.Type.ToString(),
                Status = x.Status,
                CostEstimate = x.CostEstimate,
                InspectionScheduledAt = x.InspectionScheduledAt.HasValue
                ? DateTimeHelper.ToVietnamTime(x.InspectionScheduledAt.Value)
                : null,
                CreatedAt = DateTimeHelper.ToVietnamTime(x.CreatedAt)
            }).ToList();
        }

        public async Task<ServiceRequestDetailDto> GetDetailAsync(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Không tìm thấy yêu cầu dịch vụ");
            var latestApprovedContract = await _contractRepo
                .GetLatestApprovedByGroupAndVehicleAsync(entity.GroupId, entity.VehicleId);

            return new ServiceRequestDetailDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Type = entity.Type.ToString(),
                Description = entity.Description,
                Status = entity.Status,
                VehicleName = $"{entity.Vehicle.Make} {entity.Vehicle.Model}",
                PlateNumber = entity.Vehicle.PlateNumber ?? string.Empty,
                GroupName = entity.Group.Name,
                CreatedByName = entity.CreatedByAccount.FullName,
                CostEstimate = entity.CostEstimate,
                CompletedAt = entity.CompletedAt.HasValue
                ? DateTimeHelper.ToVietnamTime(entity.CompletedAt.Value)
                : null,
                ApprovedAt = entity.ApprovedAt.HasValue
                ? DateTimeHelper.ToVietnamTime(entity.ApprovedAt.Value)
                : null,
                InspectionScheduledAt = entity.InspectionScheduledAt.HasValue
                ? DateTimeHelper.ToVietnamTime(entity.InspectionScheduledAt.Value)
                : null,
                InspectionNotes = entity.InspectionNotes,
                ServiceCenterName = entity.ServiceCenter?.Name,
                ServiceCenterAddress = entity.ServiceCenter?.Address,
                TechnicianName = entity.Technician?.FullName,
                ReportUrl = entity.Job?.ReportUrl,
                VehicleContractStatus = latestApprovedContract?.Status,
                ContractEffectiveFrom = latestApprovedContract?.EffectiveFrom,
                ContractExpiresAt = latestApprovedContract?.ExpiresAt,
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

            var vehicleName = $"{vehicle.Make} {vehicle.Model}";
            var plate = string.IsNullOrEmpty(vehicle.PlateNumber) ? "" : $" - {vehicle.PlateNumber}";

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


            var members = await _groupMemberRepo.GetByGroupIdAsync(req.GroupId);
            foreach (var m in members.Where(m => m.UserId != currentUserId))
            {
                await _notificationService.CreateAsync(
                    m.UserId,
                    "Yêu cầu dịch vụ mới",
                    $"Một thành viên đã tạo yêu cầu dịch vụ cho xe: {vehicleName}{plate}",
                    "SERVICE_REQUEST_CREATED",
                    entity.Id
                );
            }

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

            var members = await _groupMemberRepo.GetByGroupIdAsync(entity.GroupId);
            var vehicle = await _vehicleRepo.GetByIdAsync(entity.VehicleId);

            var vehicleName = $"{vehicle.Make} {vehicle.Model}";
            var plate = string.IsNullOrWhiteSpace(vehicle.PlateNumber) ? "" : $" - {vehicle.PlateNumber}";
            var schedule = entity.InspectionScheduledAt?.ToString("dd/MM/yyyy HH:mm");

            foreach (var m in members.Where(m => m.UserId != technicianId))
            {
                await _notificationService.CreateAsync(
                    m.UserId,
                    "Lịch kiểm tra xe",
                    $"Xe {vehicleName}{plate} đã được đặt lịch kiểm tra vào {schedule}.",
                    "SR_INSPECTION_SCHEDULED",
                    entity.Id
                );
            }

            return await GetDetailAsync(id);
        }

        public async Task<ServiceRequestDetailDto> ProvideCostEstimateAsync(Guid id, ProvideCostEstimateRequest req, Guid technicianId)
        {
            var entity = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Không tìm thấy yêu cầu dịch vụ");

            if (entity.TechnicianId != technicianId)
                throw new UnauthorizedAccessException("Bạn không có quyền gửi báo giá cho yêu cầu này");
            
            if (req.EstimatedFinishAt == null)
                throw new InvalidOperationException("Vui lòng nhập ngày dự kiến hoàn thành.");

            if (req.EstimatedFinishAt <= DateTime.UtcNow)
                throw new InvalidOperationException("Ngày dự kiến hoàn thành phải lớn hơn hiện tại.");
            
            entity.CostEstimate = req.CostEstimate;
            entity.CompletedAt = req.EstimatedFinishAt;
            entity.Status = "VOTING";
            entity.UpdatedAt = DateTime.UtcNow;
            entity.InspectionNotes = req.Notes;

            await _repo.UpdateAsync(entity);
            await _repo.SaveChangesAsync();

            var members = await _groupMemberRepo.GetByGroupIdAsync(entity.GroupId);
            var vehicle = await _vehicleRepo.GetByIdAsync(entity.VehicleId);

            var vehicleName = $"{vehicle.Make} {vehicle.Model}";
            var plate = string.IsNullOrWhiteSpace(vehicle.PlateNumber) ? "" : $" - {vehicle.PlateNumber}";
            var cost = entity.CostEstimate?.ToString("N0");

            foreach (var m in members.Where(m => m.UserId != technicianId))
            {
                await _notificationService.CreateAsync(
                    m.UserId,
                    "Báo giá dịch vụ",
                    $"Xe {vehicleName}{plate} đã có báo giá {cost} đồng. Vui lòng xem và biểu quyết.",
                    "SR_COST_ESTIMATE_READY",
                    entity.Id
                );
            }

            return await GetDetailAsync(id);
        }
        public async Task<IEnumerable<ServiceRequestDto>> GetMyGroupRequestsAsync(Guid currentUserId)
        {
            var list = (await _repo.GetByGroupMembersAsync(currentUserId))
            .OrderByDescending(x => x.CreatedAt); 


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

            var list = (await _repo.GetByGroupIdAsync(groupId))
                .OrderByDescending(x => x.CreatedAt); 


            return list.Select(x => new ServiceRequestDto
            {
                Id = x.Id,
                Title = x.Title,
                Type = x.Type.ToString(),
                Status = x.Status,
                CostEstimate = x.CostEstimate,
                InspectionScheduledAt = x.InspectionScheduledAt.HasValue
                ? DateTimeHelper.ToVietnamTime(x.InspectionScheduledAt.Value)
                : null,
                CreatedAt = DateTimeHelper.ToVietnamTime(x.CreatedAt)
            }).ToList();
        }

        public async Task<IEnumerable<ServiceRequestDto>> GetMyRequestsAsync(Guid currentUserId)
        {
            var list = await _repo.GetAllAsync();
            list = list.Where(x => x.CreatedBy == currentUserId)
            .OrderByDescending(x => x.CreatedAt);

            return list.Select(x => new ServiceRequestDto
            {
                Id = x.Id,
                Title = x.Title,
                Type = x.Type.ToString(),
                Status = x.Status,
                CostEstimate = x.CostEstimate,
                InspectionScheduledAt = x.InspectionScheduledAt.HasValue
                ? DateTimeHelper.ToVietnamTime(x.InspectionScheduledAt.Value)
                : null,
                CreatedAt = DateTimeHelper.ToVietnamTime(x.CreatedAt)
            }).ToList();
        }

        public async Task<IEnumerable<ServiceRequestDetailDto>> GetAssignedRequestsByUserAsync(Guid currentUserId)

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
            list = list.OrderByDescending(x => x.CreatedAt); 

            return list.Select(x => new ServiceRequestDetailDto
            {
                Id = x.Id,
                Title = x.Title,
                Type = x.Type.ToString(),
                Status = x.Status,
                Description = x.Description,
                VehicleName = $"{x.Vehicle.Make} {x.Vehicle.Model}",
                PlateNumber = x.Vehicle.PlateNumber ?? string.Empty,
                GroupName = x.Group.Name,
                CreatedByName = x.CreatedByAccount.FullName,
                CostEstimate = x.CostEstimate,
                CompletedAt = x.CompletedAt.HasValue
                ? DateTimeHelper.ToVietnamTime(x.CompletedAt.Value)
                : null,
                TechnicianName = x.Technician?.FullName,
                InspectionScheduledAt = x.InspectionScheduledAt.HasValue
                ? DateTimeHelper.ToVietnamTime(x.InspectionScheduledAt.Value)
                : null,
                CreatedAt = DateTimeHelper.ToVietnamTime(x.CreatedAt)
            }).ToList();
        }


        public async Task<(bool IsSuccess, string message ,RevenueStatisticResponse)> GetRevenueStatisticAsync(DateTime startDate, DateTime endDate)
        {
          var  startUtc = DateTimeHelper.ToUtcFromVietnamTime(startDate);
         var   endUtc = DateTimeHelper.ToUtcFromVietnamTime(endDate);
            if (startDate >= endDate)
                return (false, "Ngày bắt đầu phải trước ngày kết thúc", null);
            var orders = await _repo.GetCompletedOrdersInRangeAsync(startUtc, endUtc);

            var response = new RevenueStatisticResponse
            {
                TotalRevenue = orders.Sum(x => x.CostEstimate),
                CompletedOrders = orders.Count,
                VehiclesServiced = orders.Select(x => x.VehicleId).Distinct().Count(),
                TechnicianRevenue = orders
                    .GroupBy(x => new { x.TechnicianId, x.Technician.FullName })
                    .Select(g => new TechnicianRevenueModel
                    {
                        TechnicianName = g.Key.FullName,
                        Revenue = g.Sum(o => o.CostEstimate)
                    })
                    .OrderByDescending(x => x.Revenue)
                    .ToList()
            };

            return (true, "Thống kê doanh thu thành công", response);
        }

    }
}
