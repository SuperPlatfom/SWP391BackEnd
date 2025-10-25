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
        public ServiceRequestConfirmationService(
            IServiceRequestConfirmationRepository confirmationRepo,
            IServiceRequestRepository requestRepo,
            IGroupMemberRepository groupMemberRepo,
            IGroupExpenseService groupExpenseService,
            IGroupExpenseRepository groupExpenseRepo)
        {
            _confirmationRepo = confirmationRepo;
            _requestRepo = requestRepo;
            _groupMemberRepo = groupMemberRepo;
            _groupExpenseService = groupExpenseService;
            _groupExpenseRepo = groupExpenseRepo;
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
            


            var totalMembers = await _groupMemberRepo.CountMembersAsync(request.GroupId);
            if (totalMembers <= 0)
                throw new InvalidOperationException("Không tìm thấy danh sách đồng sở hữu trong nhóm");
            await _confirmationRepo.SaveChangesAsync();

            var allVotes = await _confirmationRepo.GetByRequestIdAsync(requestId);
            var confirmCount = allVotes.Count(v => v.Decision == "CONFIRM");
            var rejectCount = allVotes.Count(v => v.Decision == "REJECT");
            if (confirmCount == totalMembers)
            {

                var existingExpense = await _groupExpenseRepo.GetByRequestIdAsync(request.Id);

                if (existingExpense == null)
                {
                    request.Status = "APPROVED";
                    request.ApprovedAt = DateTime.UtcNow;

                    await _requestRepo.UpdateAsync(request);
                    await _requestRepo.SaveChangesAsync();


                    await _groupExpenseService.CreateFromApprovedRequestAsync(request.Id);
                }
            }
            else if (rejectCount > 0)
            {
                request.Status = "REJECTED";
                request.UpdatedAt = DateTime.UtcNow;
                await _requestRepo.UpdateAsync(request);
                await _requestRepo.SaveChangesAsync();
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
    }
}