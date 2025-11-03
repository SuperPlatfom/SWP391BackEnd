using BusinessObject.DTOs.RequestModels;
using BusinessObject.Models;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ServiceRequestConfirmationService : IServiceRequestConfirmationService
    {
        private readonly IServiceRequestConfirmationRepository _confirmationRepo;
        private readonly IServiceRequestRepository _requestRepo;
        private readonly IGroupMemberRepository _groupMemberRepo;
        private readonly IGroupExpenseService _groupExpenseService;
        private readonly IGroupExpenseRepository _groupExpenseRepo;
        private readonly INotificationService _noti;
        private readonly IAccountRepository _accountRepo;
        private readonly IVehicleRepository _vehicleRepo;
        public ServiceRequestConfirmationService(
            IServiceRequestConfirmationRepository confirmationRepo,
            IServiceRequestRepository requestRepo,
            IGroupMemberRepository groupMemberRepo,
            IGroupExpenseService groupExpenseService,
            IGroupExpenseRepository groupExpenseRepo,
            INotificationService noti,
            IAccountRepository accountRepo,
            IVehicleRepository vehicleRepo)
        {
            _confirmationRepo = confirmationRepo;
            _requestRepo = requestRepo;
            _groupMemberRepo = groupMemberRepo;
            _groupExpenseService = groupExpenseService;
            _groupExpenseRepo = groupExpenseRepo;
            _noti = noti;
            _accountRepo = accountRepo;
            _vehicleRepo = vehicleRepo;
        }

        public async Task<ServiceRequestConfirmationDto> ConfirmAsync(Guid currentUserId, Guid requestId, bool confirm, string? reason)
        {
            var request = await _requestRepo.GetByIdAsync(requestId)
                ?? throw new KeyNotFoundException("Không tìm thấy yêu cầu dịch vụ");

            if (request.Status != "VOTING")
                throw new InvalidOperationException("Chỉ được biểu quyết khi yêu cầu đang ở trạng thái VOTING");

            var isMember = await _groupMemberRepo.IsMemberAsync(request.GroupId, currentUserId);
            if (!isMember)
                throw new UnauthorizedAccessException("Bạn không thuộc nhóm sở hữu xe này");

            var existing = await _confirmationRepo.GetByUserAsync(requestId, currentUserId);
            if (existing != null)
                throw new InvalidOperationException("Bạn đã biểu quyết cho yêu cầu này rồi");

            var confirmation = new ServiceRequestConfirmation
            {
                Id = Guid.NewGuid(),
                RequestId = requestId,
                UserId = currentUserId,
                Decision = confirm ? "CONFIRM" : "REJECT",
                Reason = reason,
                DecidedAt = DateTime.UtcNow
            };

            await _confirmationRepo.AddAsync(confirmation);
            await _confirmationRepo.SaveChangesAsync();

            var voter = await _accountRepo.GetByIdAsync(currentUserId);
            var voterName = voter?.FullName ?? "Thành viên";

            var members = await _groupMemberRepo.GetByGroupIdAsync(request.GroupId);
            var vehicle = await _vehicleRepo.GetByIdAsync(request.VehicleId);
            var vehicleName = $"{vehicle.Make} {vehicle.Model}";
            var plate = string.IsNullOrWhiteSpace(vehicle.PlateNumber) ? "" : $" - {vehicle.PlateNumber}";

            foreach (var m in members.Where(x => x.UserId != currentUserId))
            {
                await _noti.CreateAsync(
                    m.UserId,
                    "Biểu quyết yêu cầu dịch vụ",
                    $"{voterName} đã {(confirm ? "đồng ý" : "từ chối")} yêu cầu cho xe {vehicleName}{plate}",
                    "SR_VOTE_UPDATE",
                    request.Id
                );
            }

            var totalMembers = members.Count();
            var allVotes = await _confirmationRepo.GetByRequestIdAsync(requestId);
            var confirmCount = allVotes.Count(v => v.Decision == "CONFIRM");
            var rejectCount = allVotes.Count(v => v.Decision == "REJECT");

            if (confirmCount == totalMembers)
            {
                request.Status = "APPROVED";
                request.ApprovedAt = DateTime.UtcNow;
                await _requestRepo.UpdateAsync(request);
                await _requestRepo.SaveChangesAsync();

                await _groupExpenseService.CreateFromApprovedRequestAsync(request.Id);

                foreach (var m in members)
                {
                    await _noti.CreateAsync(
                        m.UserId,
                        "Yêu cầu dịch vụ được phê duyệt ✅",
                        $"Tất cả thành viên đã đồng ý yêu cầu dịch vụ cho xe {vehicleName}{plate}",
                        "SR_REQUEST_APPROVED",
                        request.Id
                    );
                }
            }
            else if (rejectCount > 0)
            {
                request.Status = "REJECTED";
                request.UpdatedAt = DateTime.UtcNow;
                await _requestRepo.UpdateAsync(request);
                await _requestRepo.SaveChangesAsync();

                foreach (var m in members)
                {
                    await _noti.CreateAsync(
                        m.UserId,
                        "Yêu cầu dịch vụ bị từ chối ❌",
                        $"{voterName} đã từ chối yêu cầu dịch vụ cho xe {vehicleName}{plate}",
                        "SR_REQUEST_REJECTED",
                        request.Id
                    );
                }
            }

            return new ServiceRequestConfirmationDto
            {
                Id = confirmation.Id,
                RequestId = confirmation.RequestId,
                UserId = confirmation.UserId,
                Decision = confirmation.Decision,
                Reason = confirmation.Reason,
                DecidedAt = confirmation.DecidedAt
            };
        }



        public async Task<IEnumerable<ServiceRequestConfirmationDto>> GetByRequestIdAsync(Guid requestId)
        {
            var list = await _confirmationRepo.GetByRequestIdAsync(requestId);

            return list.Select(c => new ServiceRequestConfirmationDto
            {
                Id = c.Id,
                RequestId = c.RequestId,
                UserId = c.UserId,
                Decision = c.Decision,
                Reason = c.Reason,
                DecidedAt = c.DecidedAt
            }).ToList();
        }

        public async Task<IEnumerable<ServiceRequestConfirmationDto>> GetByUserAsync(Guid currentUserId)
        {
            var list = await _confirmationRepo.GetAllByUserAsync(currentUserId);

            return list.Select(c => new ServiceRequestConfirmationDto
            {
                Id = c.Id,
                RequestId = c.RequestId,
                UserId = c.UserId,
                Decision = c.Decision,
                Reason = c.Reason,
                DecidedAt = c.DecidedAt
            }).ToList();
        }
        public async Task<IEnumerable<ServiceRequestVoteStatusDto>> GetVoteStatusAsync(Guid requestId)
        {
            var request = await _requestRepo.GetByIdAsync(requestId)
                ?? throw new KeyNotFoundException("Không tìm thấy yêu cầu dịch vụ");

            var members = await _groupMemberRepo.GetByGroupIdAsync(request.GroupId);
            var votes = await _confirmationRepo.GetByRequestIdAsync(requestId);

            var result = from m in members
                         join v in votes on m.UserId equals v.UserId into mv
                         from vote in mv.DefaultIfEmpty()
                         select new ServiceRequestVoteStatusDto
                         {
                             UserId = m.UserId,
                             FullName = m.UserAccount.FullName,
                             Decision = vote?.Decision ?? "PENDING",
                             DecidedAt = vote?.DecidedAt
                         };

            return result.ToList();
        }


    }
}