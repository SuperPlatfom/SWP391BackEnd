using BusinessObject.DTOs.ResponseModels;
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
    public class GroupExpenseService : IGroupExpenseService
    {
        private readonly IGroupExpenseRepository _expenseRepo;
        private readonly IServiceRequestRepository _requestRepo;
        private readonly IEContractMemberShareRepository _shareRepo;
        private readonly IMemberInvoiceRepository _invoiceRepo;
        private readonly IEContractRepository _contractRepo;

        public GroupExpenseService(
            IGroupExpenseRepository expenseRepo,
            IServiceRequestRepository requestRepo,
            IEContractMemberShareRepository shareRepo,
            IMemberInvoiceRepository invoiceRepo,
            IEContractRepository contractRepo)
        {
            _expenseRepo = expenseRepo;
            _requestRepo = requestRepo;
            _shareRepo = shareRepo;
            _invoiceRepo = invoiceRepo;
            _contractRepo = contractRepo;
        }

        public async Task<GroupExpenseDto> CreateFromApprovedRequestAsync(Guid serviceRequestId)
        {

            var request = await _requestRepo.GetByIdAsync(serviceRequestId)
                ?? throw new KeyNotFoundException("Không tìm thấy yêu cầu dịch vụ");
            if (request.Status != "APPROVED")
                throw new InvalidOperationException("Yêu cầu dịch vụ chưa được duyệt hoàn toàn");
            if (request.CostEstimate is null)
                throw new InvalidOperationException("Yêu cầu dịch vụ chưa có báo giá");

   
            var existed = await _expenseRepo.GetByRequestIdAsync(request.Id);
            if (existed != null)
            {
                return new GroupExpenseDto
                {
                    Id = existed.Id,
                    GroupId = existed.GroupId,
                    ServiceRequestId = existed.ServiceRequestId,
                    Amount = existed.Amount,
                    Description = existed.Description,
                    IncurredAt = existed.IncurredAt,
                    Status = existed.Status
                };
            }


            var contract = await _contractRepo.GetLatestApprovedByGroupAndVehicleAsync(request.GroupId, request.VehicleId)
                ?? throw new InvalidOperationException("Không tìm thấy hợp đồng APPROVED hợp lệ cho chiếc xe này.");


            var rawShares = await _shareRepo.GetByContractIdAsync(contract.Id);
            if (rawShares == null || rawShares.Count == 0)
                throw new InvalidOperationException("Hợp đồng chưa có tỷ lệ sở hữu để chia chi phí.");

 
            var shares = rawShares
                .GroupBy(s => s.UserId)
                .Select(g => new { UserId = g.Key, OwnershipRate = g.Sum(x => x.OwnershipRate ?? 0m) })
                .ToList();

            var totalRate = shares.Sum(s => s.OwnershipRate);
            if (Math.Abs(totalRate - 100m) > 0.0001m)
                throw new InvalidOperationException($"Tổng tỷ lệ sở hữu không bằng 100% (hiện = {totalRate}%).");

            var totalAmount = request.CostEstimate.Value;
            if (totalAmount < shares.Count * 1000)
                throw new InvalidOperationException("Tổng số tiền quá nhỏ, không thể chia (tối thiểu 1.000₫ mỗi người).");

            var memberAmounts = new List<(Guid UserId, decimal Amount)>();
            decimal totalAllocated = 0;
            foreach (var s in shares)
            {
                var amount = Math.Floor(totalAmount * s.OwnershipRate / 100);
                if (amount < 1000)
                    throw new InvalidOperationException($"Phần chia của thành viên ({s.OwnershipRate}%) nhỏ hơn 1.000₫.");
                totalAllocated += amount;
                memberAmounts.Add((s.UserId, amount));
            }

   
            var diff = totalAmount - totalAllocated;
            if (diff > 0 && memberAmounts.Count > 0)
            {
                var topOwnerId = shares.OrderByDescending(x => x.OwnershipRate).First().UserId;
                var idx = memberAmounts.FindIndex(m => m.UserId == topOwnerId);
                if (idx >= 0) memberAmounts[idx] = (memberAmounts[idx].UserId, memberAmounts[idx].Amount + diff);
            }

 
            var expense = new GroupExpense
            {
                Id = Guid.NewGuid(),
                GroupId = request.GroupId,
                ServiceRequestId = request.Id,
                Amount = totalAmount,
                Description = $"Chi phí cho dịch vụ {request.Title}",
                Status = "CONFIRMED",
                IncurredAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _expenseRepo.AddAsync(expense);
            await _expenseRepo.SaveChangesAsync();


            foreach (var (userId, amount) in memberAmounts)
            {
                var existedInv = await _invoiceRepo.GetByExpenseAndUserAsync(expense.Id, userId);
                if (existedInv != null) continue;

                var percent = shares.First(s => s.UserId == userId).OwnershipRate;
                var invoice = new MemberInvoice
                {
                    Id = Guid.NewGuid(),
                    ExpenseId = expense.Id,
                    GroupId = request.GroupId,
                    UserId = userId,
                    Title = $"Invoice dịch vụ {request.Title}",
                    Description = $"Chi phí chia theo tỉ lệ sở hữu ({percent}%)",
                    TotalAmount = amount,
                    AmountPaid = 0,
                    Status = "DUE",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _invoiceRepo.AddAsync(invoice);
            }
            await _invoiceRepo.SaveChangesAsync();

            return new GroupExpenseDto
            {
                Id = expense.Id,
                GroupId = expense.GroupId,
                ServiceRequestId = expense.ServiceRequestId,
                Amount = expense.Amount,
                Description = expense.Description,
                IncurredAt = expense.IncurredAt,
                Status = expense.Status
            };
        }





        public async Task<IEnumerable<GroupExpenseDto>> GetByGroupAsync(Guid groupId)
        {
            var list = await _expenseRepo.GetByGroupAsync(groupId);

            return list.Select(x => new GroupExpenseDto
            {
                Id = x.Id,
                GroupId = x.GroupId,
                Amount = x.Amount,
                Description = x.Description,
                IncurredAt = x.IncurredAt,
                Status = x.Status
            }).ToList();
        }
    }
}
