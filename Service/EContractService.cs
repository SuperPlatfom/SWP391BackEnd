using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Service.Helpers;
using Service.Interfaces;

namespace Service
{
    public class EContractService : IContractService
    {
        private readonly IEContractRepository _contractRepo;
        private readonly IEContractSignerRepository _signerRepo;
        private readonly IEContractMemberShareRepository _shareRepo;
        private readonly ICoOwnershipGroupRepository _groupRepo;
        private readonly IVehicleRepository _vehicleRepo;
        private readonly IContractTemplateRepository _templateRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly IGroupMemberRepository _groupMemberRepo;
        private readonly IMustacheRenderer _renderer;

        public EContractService(
            IEContractRepository contractRepo,
            IEContractSignerRepository signerRepo,
            IEContractMemberShareRepository shareRepo,
            ICoOwnershipGroupRepository groupRepo,
            IVehicleRepository vehicleRepo,
            IContractTemplateRepository templateRepo,
            IAccountRepository accountRepo,
            IGroupMemberRepository groupMemberRepo,
            IMustacheRenderer renderer)
        {
            _contractRepo = contractRepo;
            _signerRepo = signerRepo;
            _shareRepo = shareRepo;
            _groupRepo = groupRepo;
            _vehicleRepo = vehicleRepo;
            _templateRepo = templateRepo;
            _accountRepo = accountRepo;
            _groupMemberRepo = groupMemberRepo;
            _renderer = renderer;
        }

        public async Task<ContractDetailDto> CreateAsync(CreateContractRequest req, Guid currentUserId)
        {

            var group = await _groupRepo.GetByIdAsync(req.GroupId)
                        ?? throw new KeyNotFoundException("Group not found");
            var vehicle = await _vehicleRepo.GetByIdAsync(req.VehicleId)
                        ?? throw new KeyNotFoundException("Vehicle not found");
            var template = await _templateRepo.GetByIdAsync(req.TemplateId)
                        ?? throw new KeyNotFoundException("Template not found");

            if (req.ExpiresAt.HasValue && req.ExpiresAt <= req.EffectiveFrom)
                throw new ArgumentException("Expiration date must be after effective date");


            var members = await _groupMemberRepo.GetByGroupIdAsync(req.GroupId);
            bool isMemberAccepted = members.Any(m => m.UserId == currentUserId && m.InviteStatus == "ACCEPTED");
            bool isOwner = group.CreatedBy == currentUserId;

            if (!(isOwner || isMemberAccepted))
                throw new UnauthorizedAccessException("Bạn không có quyền tạo hợp đồng cho nhóm này.");

            var contract = new EContract
            {
                Id = Guid.NewGuid(),
                TemplateId = req.TemplateId,
                GroupId = req.GroupId,
                VehicleId = req.VehicleId,
                Title = string.IsNullOrWhiteSpace(req.Title)
                    ? $"Hợp đồng đồng sở hữu - {vehicle.Make} {vehicle.Model}"
                    : req.Title,
                Status = "DRAFT",
                EffectiveFrom = req.EffectiveFrom,
                ExpiresAt = req.ExpiresAt,
                CreatedBy = currentUserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _contractRepo.AddAsync(contract);

            var acceptedMembers = members.Where(m => m.InviteStatus == "ACCEPTED").ToList();
            var signers = acceptedMembers.Select(m => new EContractSigner
            {
                Id = Guid.NewGuid(),
                ContractId = contract.Id,
                UserId = m.UserId,
                Status = "PENDING",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }).ToList();
            await _signerRepo.AddRangeAsync(signers);

            if (req.OwnershipShares?.Any() == true)
            {

                var total = req.OwnershipShares.Sum(x => x.Rate);
                if (Math.Round(total, 2) != 100m)
                    throw new InvalidOperationException("Tổng tỷ lệ sở hữu phải bằng 100%");


                var acceptedUserIds = acceptedMembers.Select(m => m.UserId).ToHashSet();
                var invalidUser = req.OwnershipShares.FirstOrDefault(x => !acceptedUserIds.Contains(x.UserId));
                if (invalidUser != null)
                    throw new InvalidOperationException("Có người nhận tỷ lệ sở hữu không thuộc nhóm hoặc chưa được chấp nhận.");


                if (req.OwnershipShares.Select(x => x.UserId).Distinct().Count() != req.OwnershipShares.Count)
                    throw new InvalidOperationException("OwnershipShares có user trùng lặp.");

                var shareEntities = req.OwnershipShares.Select(x => new EContractMemberShare
                {
                    Id = Guid.NewGuid(),
                    ContractId = contract.Id,
                    UserId = x.UserId,
                    OwnershipRate = x.Rate,
                    CreatedAt = DateTime.UtcNow
                }).ToList();
                await _shareRepo.AddRangeAsync(shareEntities);
            }
            await _contractRepo.SaveChangesAsync();
            var contractDetail = await _contractRepo.GetDetailAsync(contract.Id)
                ?? throw new KeyNotFoundException("Contract not found for content rendering");

            var htmlData = new
            {
                ContractDate = DateTimeHelper.ToVietnamTime(contract.CreatedAt).ToString("dd/MM/yyyy"),
                EffectiveFrom = DateTimeHelper.ToVietnamTime(contract.EffectiveFrom ?? contract.CreatedAt).ToString("dd/MM/yyyy"),
                ExpiresAt = contract.ExpiresAt.HasValue ? DateTimeHelper.ToVietnamTime(contract.ExpiresAt.Value).ToString("dd/MM/yyyy") : "",
                VehicleName = vehicle.Make + " " + vehicle.Model,
                LicensePlate = vehicle.PlateNumber,
                BatteryCapacity = vehicle.BatteryCapacityKwh,
                RangeKm = vehicle.RangeKm,
                Color = vehicle.Color,
                CoOwnersCount = members.Count(),
                CoOwners = members.Select(m => new {
                    FullName = m.UserAccount.FullName,
                    CitizenId = m.UserAccount.CitizenIdentityCard.IdNumber,
                    Address = m.UserAccount.CitizenIdentityCard.Address,
                    OwnershipRate = req.OwnershipShares?.FirstOrDefault(x => x.UserId == m.UserId)?.Rate ?? 0,
                    Email = m.UserAccount.Email,
                    Phone = m.UserAccount.Phone
                }).ToList(),
                Clauses = template.Clauses.OrderBy(c => c.OrderIndex).Select(c => new { c.Title, c.Body }).ToList(),
                CreatedByName = (await _accountRepo.GetByIdAsync(currentUserId))?.FullName ?? ""
            };

            contract.Content = _renderer.Render(template.Content, htmlData);
            await _contractRepo.SaveChangesAsync();


            return await BuildDetailDto(contract.Id);
        }



        public async Task<IEnumerable<ContractSummaryDto>> GetAllAsync(Guid currentUserId, string? status = null, Guid? groupId = null)
        {
            var user = await _accountRepo.GetByIdAsync(currentUserId)
                       ?? throw new KeyNotFoundException("User not found");

            var q = await _contractRepo.QueryAsync();

            var isAdmin = user.Role?.Name?.Equals("Admin", StringComparison.OrdinalIgnoreCase) == true;
            if (!isAdmin)
            {
                var relatedIds = await _signerRepo.GetContractIdsByUserAsync(currentUserId);
                q = q.Where(c => c.CreatedBy == currentUserId || relatedIds.Contains(c.Id));
            }

            if (!string.IsNullOrEmpty(status))
                q = q.Where(c => c.Status == status);

            if (groupId.HasValue)
                q = q.Where(c => c.GroupId == groupId.Value);

            return await q.OrderByDescending(c => c.CreatedAt)
                          .Select(c => new ContractSummaryDto
                          {
                              Id = c.Id,
                              Title = c.Title,
                              TemplateName = c.Template.Name,
                              GroupName = c.Group.Name,
                              VehicleName = c.Vehicle.Make + " " + c.Vehicle.Model,
                              Status = c.Status,
                              CreatedAt = DateTimeHelper.ToVietnamTime(c.CreatedAt)
                          })
                          .ToListAsync();

        }

        public async Task<ContractDetailDto> GetDetailAsync(Guid id, Guid currentUserId)
        {
            return await BuildDetailDto(id);
        }


        public async Task<string> GetPreviewHtmlAsync(Guid id, Guid currentUserId)
        {
            var contract = await _contractRepo.GetDetailAsync(id)
                           ?? throw new KeyNotFoundException("Contract not found");
            var template = contract.Template;


            var sharesMap = contract.MemberShares?
                .ToDictionary(ms => ms.UserId, ms => ms.OwnershipRate ?? 0m)
                ?? new Dictionary<Guid, decimal>();

            var coOwnerRows = contract.Signers.Select(s =>
            {
                var u = s.User;
                var idcard = u?.CitizenIdentityCard;
                return new
                {
                    FullName = u?.FullName ?? "",
                    CitizenId = idcard?.IdNumber ?? "",
                    Address = idcard?.Address ?? "",
                    OwnershipRate = sharesMap.TryGetValue(s.UserId, out var r) ? r : 0,
                    Email = u?.Email ?? "",
                    Phone = u?.Phone ?? "",
                    ConfirmedAt = s.OtpVerifiedAt.HasValue
                        ? DateTimeHelper.ToVietnamTime(s.OtpVerifiedAt.Value).ToString("dd/MM/yyyy HH:mm") : "",
                    OtpCode = s.OtpVerifiedAt.HasValue ? "ĐÃ XÁC NHẬN" : ""
                };
            }).ToList();

            var clauseRows = template.Clauses
                .OrderBy(c => c.OrderIndex)
                .Select(c => new { c.Title, c.Body })
                .ToList();

            var data = new
            {
                ContractDate = DateTimeHelper.ToVietnamTime(contract.CreatedAt).ToString("dd/MM/yyyy"),
                EffectiveFrom = DateTimeHelper.ToVietnamTime(contract.EffectiveFrom ?? contract.CreatedAt).ToString("dd/MM/yyyy"),
                ExpiresAt = contract.ExpiresAt.HasValue ? DateTimeHelper.ToVietnamTime(contract.ExpiresAt.Value).ToString("dd/MM/yyyy") : "",
                VehicleName = contract.Vehicle?.Make + " " + contract.Vehicle?.Model,
                LicensePlate = contract.Vehicle?.PlateNumber,
                BatteryCapacity = contract.Vehicle?.BatteryCapacityKwh,
                RangeKm = contract.Vehicle?.RangeKm,
                Color = contract.Vehicle?.Color,
                CoOwnersCount = coOwnerRows.Count,
                CoOwners = coOwnerRows,
                Clauses = clauseRows,
                CreatedByName = contract.CreatedByAccount?.FullName ?? "",
            };

            return _renderer.Render(template.Content, data);
        }

        public Task SendOtpAsync(Guid contractId, Guid signerUserId)
        {
            // 1) validate signer thuộc contract
            // 2) generate OTP + save to signer
            // 3) send mail
            // ... gọi repository signer + mail service (nếu có)
            return Task.CompletedTask;
        }

        public Task VerifyOtpAsync(Guid contractId, Guid signerUserId, string otp)
        {
            // 1) validate OTP còn hạn
            // 2) update signer.Status = CONFIRMED + OtpVerifiedAt
            // 3) nếu đủ chữ ký -> chuyển status PENDING_REVIEW
            return Task.CompletedTask;
        }

        public async Task ReviewAsync(Guid contractId, Guid staffId, bool approve, string? note)
        {
            var c = await _contractRepo.GetDetailAsync(contractId)
                    ?? throw new KeyNotFoundException("Contract not found");

            c.ReviewedAt = DateTime.UtcNow;
            c.ReviewedBy = staffId;
            c.ReviewNote = note;
            c.Status = approve ? "APPROVED" : "REJECTED";
            await _contractRepo.SaveChangesAsync();

        }

        public async Task UpdateStatusAsync(Guid contractId, string newStatus)
        {
            var c = await _contractRepo.GetDetailAsync(contractId)
                    ?? throw new KeyNotFoundException("Contract not found");
            c.Status = newStatus;
            c.UpdatedAt = DateTime.UtcNow;
            await _contractRepo.SaveChangesAsync();
        }

        public Task<byte[]> GetFinalizedPdfAsync(Guid contractId)
        {
            // 1) Load detail
            // 2) Render HTML -> convert PDF (wkhtmltopdf/QuestPDF/…)
            // 3) trả về bytes; hoặc lưu S3 & trả link
            return Task.FromResult(Array.Empty<byte>());
        }

        private async Task<ContractDetailDto> BuildDetailDto(Guid id)
        {
            var c = await _contractRepo.GetDetailAsync(id)
                    ?? throw new KeyNotFoundException("Contract not found");

            var shares = c.MemberShares?.Select(ms => new OwnershipShareDto
            {
                UserId = ms.UserId,
                FullName = ms.User.FullName,
                Rate = ms.OwnershipRate ?? 0
            }).ToList() ?? new List<OwnershipShareDto>();


            var permissions = new PermissionDto
            {
                CanSign = c.Status == "SIGNING",            
                CanSendOtp = c.Status == "DRAFT" || c.Status == "SIGNING",
                CanReview = c.Status == "PENDING_REVIEW",
                CanDownload = true,
                CanEditShares = c.Status == "DRAFT"
            };

            return new ContractDetailDto
            {
                Id = c.Id,
                Title = c.Title,
                Status = c.Status,
                Content = c.Content,                               
                FileUrl = c.FileUrl,
                EffectiveFrom = c.EffectiveFrom,
                ExpiresAt = c.ExpiresAt,
                CreatedAt = c.CreatedAt,

                TemplateId = c.TemplateId,
                TemplateName = c.Template.Name,
                TemplateVersion = c.Template.Version,

                GroupId = c.GroupId,
                GroupName = c.Group.Name,

                VehicleId = c.VehicleId,
                PlateNumber = c.Vehicle.PlateNumber,
                VehicleName = (c.Vehicle.Make + " " + c.Vehicle.Model).Trim(),
                ModelYear = c.Vehicle.ModelYear,
                Color = c.Vehicle.Color,
                BatteryCapacityKwh = c.Vehicle.BatteryCapacityKwh,
                RangeKm = c.Vehicle.RangeKm,

                Signers = c.Signers.Select(s => new ContractSignerDto
                {
                    UserId = s.UserId,
                    FullName = s.User.FullName,
                    Email = s.User.Email,
                    Status = s.Status,
                    OtpSentAt = s.OtpSentAt.HasValue ? DateTimeHelper.ToVietnamTime(s.OtpSentAt.Value) : null,
                    OtpVerifiedAt = s.OtpVerifiedAt.HasValue ? DateTimeHelper.ToVietnamTime(s.OtpVerifiedAt.Value) : null,
                }).ToList(),

                OwnershipShares = shares,

                Audit = new AuditDto
                {
                    CreatedBy = c.CreatedBy,
                    CreatedByName = c.CreatedByAccount?.FullName ?? "",
                    SignedAt = c.SignedAt,
                    ReviewedAt = c.ReviewedAt,
                    ReviewedBy = c.ReviewedBy,
                    ReviewNote = c.ReviewNote
                },

                Permissions = permissions,

            };
        }

    }
}
