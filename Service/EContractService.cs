using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Enums;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Repository.Interfaces;
using Service.Helpers;
using Service.Interfaces;
using System.Security.Cryptography;
using System.Text;

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
        private readonly IMailService _mailService;
        private readonly IConfiguration _config;
        private readonly IFirebaseStorageService _firebaseStorageService;
        private readonly INotificationService _noti;

        public EContractService(
            IEContractRepository contractRepo,
            IEContractSignerRepository signerRepo,
            IEContractMemberShareRepository shareRepo,
            ICoOwnershipGroupRepository groupRepo,
            IVehicleRepository vehicleRepo,
            IContractTemplateRepository templateRepo,
            IAccountRepository accountRepo,
            IGroupMemberRepository groupMemberRepo,
            IMustacheRenderer renderer,
            IMailService mailService,
            IConfiguration config,
            IFirebaseStorageService firebaseStorageService,
            INotificationService noti)
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
            _mailService = mailService;
            _config = config;
            _firebaseStorageService = firebaseStorageService;
            _noti = noti;
        }

        public async Task<ContractDetailDto> CreateAsync(CreateContractRequest req, Guid currentUserId)
        {

            var group = await _groupRepo.GetByIdAsync(req.GroupId)
                        ?? throw new KeyNotFoundException("Group not found");
            var vehicle = await _vehicleRepo.GetByIdAsync(req.VehicleId)
                        ?? throw new KeyNotFoundException("Vehicle not found");
            var template = await _templateRepo.GetByIdAsync(req.TemplateId)
                        ?? throw new KeyNotFoundException("Template not found");

            var q = await _contractRepo.QueryAsync(); 
            var hasExisting = await q.AnyAsync(c =>
                c.VehicleId == req.VehicleId &&
                c.Status != ContractStatus.Canceled &&   
                c.Status != ContractStatus.Rejected      
            );

            if (hasExisting)
            {
                var label = !string.IsNullOrWhiteSpace(vehicle.PlateNumber)
                    ? vehicle.PlateNumber
                    : $"{vehicle.Make} {vehicle.Model}";

                throw new InvalidOperationException(
                    $"Xe {label} hiện đã có hợp đồng đang tồn tại."
                );
            }

            if (req.ExpiresAt.HasValue && req.ExpiresAt <= DateTime.UtcNow)
                throw new ArgumentException("Ngày hết hạn phải sau thời điểm hiện tại");


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

            var vehicleName = $"{vehicle.Make} {vehicle.Model}";
            var plate = string.IsNullOrEmpty(vehicle.PlateNumber) ? "" : $" - {vehicle.PlateNumber}";

            foreach (var signer in signers)
            {
                await _noti.CreateAsync(
                    signer.UserId,
                    "Mời ký hợp đồng",
                    $"Bạn được mời ký hợp đồng sở hữu xe {vehicle.Make} {vehicle.Model}{(string.IsNullOrEmpty(vehicle.PlateNumber) ? "" : $" - {vehicle.PlateNumber}")}",
                    "ECONTRACT_SIGN_REQUEST",
                    contract.Id
                );
            }


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
                      
            foreach (var m in members.Where(m => m.UserId != currentUserId))
            {
                await _noti.CreateAsync(
                    m.UserId,
                    "Hợp đồng đồng sở hữu mới",
                    $"Một hợp đồng sở hữu cho xe {vehicleName}{plate} vừa được tạo.",
                    "ECONTRACT_CREATED",
                    contract.Id
                );
            }


            return await BuildDetailDto(contract.Id);
        }

        public async Task<ContractDetailDto> UpdateAsync(Guid id, CreateContractRequest req, Guid currentUserId)
        {

            var contract = await _contractRepo.GetDetailAsync(id)
                            ?? throw new KeyNotFoundException("Không tìm thấy hợp đồng");


            if (contract.CreatedBy != currentUserId)
                throw new UnauthorizedAccessException("Bạn không có quyền sửa hợp đồng này");


            if (contract.Status != ContractStatus.Draft.ToString())
                throw new InvalidOperationException("Chỉ được chỉnh sửa hợp đồng ở trạng thái DRAFT");

            if (req.ExpiresAt.HasValue && req.ExpiresAt <= contract.EffectiveFrom)
                throw new InvalidOperationException("Ngày hết hạn phải sau ngày hiệu lực");


            contract.Title = string.IsNullOrWhiteSpace(req.Title) ? contract.Title : req.Title;
            contract.ExpiresAt = req.ExpiresAt;
            contract.UpdatedAt = DateTime.UtcNow;

            if (req.VehicleId != contract.VehicleId)
                contract.VehicleId = req.VehicleId;

            if (req.TemplateId != contract.TemplateId)
                contract.TemplateId = req.TemplateId;


            if (req.OwnershipShares?.Any() == true)
            {
                var total = req.OwnershipShares.Sum(x => x.Rate);
                if (total != 100)
                    throw new InvalidOperationException("Tổng tỷ lệ sở hữu phải bằng 100%");


                await _shareRepo.DeleteByContractIdAsync(contract.Id);


                var newShares = req.OwnershipShares.Select(x => new EContractMemberShare
                {
                    Id = Guid.NewGuid(),
                    ContractId = contract.Id,
                    UserId = x.UserId,
                    OwnershipRate = x.Rate,
                    CreatedAt = DateTime.UtcNow
                }).ToList();

                await _shareRepo.AddRangeAsync(newShares);
            }


            var groupMembers = await _groupMemberRepo.GetByGroupIdAsync(contract.GroupId);
            if (groupMembers != null && groupMembers.Any())
            {

                await _signerRepo.DeleteByContractIdAsync(contract.Id);


                var signers = groupMembers.Select(gm => new EContractSigner
                {
                    Id = Guid.NewGuid(),
                    ContractId = contract.Id,
                    UserId = gm.UserId,
                    Status = "PENDING",
                    CreatedAt = DateTime.UtcNow
                }).ToList();

                await _signerRepo.AddRangeAsync(signers);
            }


            await _contractRepo.SaveChangesAsync();


            return await BuildDetailDto(contract.Id);
        }



        public async Task<IEnumerable<ContractSummaryDto>> GetAllAsync(Guid currentUserId, string? status = null, Guid? groupId = null)
        {
            var user = await _accountRepo.GetByIdAsync(currentUserId)
                       ?? throw new KeyNotFoundException("User not found");

            var q = await _contractRepo.QueryAsync();

            var roleName = user.Role?.Name?.ToLower();
            bool isAdmin = roleName == "admin";
            bool isStaff = roleName == "staff";
            
            if (!(isAdmin || isStaff))
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
                                 CreatedAt = DateTimeHelper.ToVietnamTime(c.CreatedAt),
                                 EffectiveFrom = c.EffectiveFrom,
                                 ExpiresAt = c.ExpiresAt
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

            string reviewedByName = "";
            string reviewedAtText = "";
            if (contract.ReviewedBy != null && contract.ReviewedBy != Guid.Empty)
            {
                var reviewer = await _accountRepo.GetByIdAsync(contract.ReviewedBy!.Value);
                reviewedByName = reviewer?.FullName ?? "";
                if (contract.ReviewedAt.HasValue)
                    reviewedAtText = DateTimeHelper.ToVietnamTime(contract.ReviewedAt.Value).ToString("dd/MM/yyyy HH:mm");
            }


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
                ReviewedByName = reviewedByName,
                ReviewedAtText = reviewedAtText,
                ReviewNote = contract.ReviewNote ?? ""
            };

            return _renderer.Render(template.Content, data);
        }

        public async Task SendOtpAsync(Guid contractId, Guid signerUserId)
        {
            var contract = await _contractRepo.GetDetailAsync(contractId)
                          ?? throw new KeyNotFoundException("Không tìm thấy hợp đồng");

            if (contract.Status != "DRAFT" && contract.Status != "SIGNING")
                throw new InvalidOperationException("Chỉ có thể gửi OTP khi hợp đồng ở trạng thái DRAFT hoặc SIGNING.");

            var signer = contract.Signers.FirstOrDefault(s => s.UserId == signerUserId)
                         ?? throw new UnauthorizedAccessException("Bạn không thuộc danh sách người ký của hợp đồng này.");

            if (signer.OtpSentAt.HasValue && (DateTime.UtcNow - signer.OtpSentAt.Value).TotalSeconds < 60)
                throw new InvalidOperationException("Vui lòng chờ 1 phút trước khi gửi lại OTP.");

            string otp = GenerateOtpCode();

            signer.OtpCode = otp;
            signer.CodeExpiry = DateTime.UtcNow.AddMinutes(10);
            signer.OtpSentAt = DateTime.UtcNow;
            signer.Status = "PENDING";
            await _signerRepo.UpdateAsync(signer);

            string subject = $"Mã OTP ký hợp đồng \"{contract.Title}\"";
            string body = $@"
        <p>Xin chào {signer.User.FullName},</p>
        <p>Mã OTP để ký hợp đồng <b>{contract.Title}</b> là: <b style='color:#007bff;font-size:18px'>{otp}</b></p>
        <p>OTP có hiệu lực trong <b>10 phút</b>. Vui lòng không chia sẻ cho người khác.</p>
        <hr/>
        <p>Thời gian gửi: {DateTimeHelper.ToVietnamTime(DateTime.UtcNow):dd/MM/yyyy HH:mm:ss}</p>";
            await _mailService.SendEmailVerificationCode(signer.User.Email, subject, body);

            if (contract.Status == "DRAFT")
            {
                contract.Status = "SIGNING";
                await _contractRepo.UpdateAsync(contract);
            }
        }


        public async Task VerifyOtpAsync(Guid contractId, Guid signerUserId, string otp)
        {
            var contract = await _contractRepo.GetDetailAsync(contractId)
                          ?? throw new KeyNotFoundException("Không tìm thấy hợp đồng");

            var signer = contract.Signers.FirstOrDefault(s => s.UserId == signerUserId)
                         ?? throw new UnauthorizedAccessException("Bạn không thuộc danh sách người ký của hợp đồng này.");

            if (string.IsNullOrWhiteSpace(otp))
                throw new InvalidOperationException("Vui lòng nhập mã OTP.");

            if (signer.CodeExpiry == null || signer.CodeExpiry < DateTime.UtcNow)
                throw new InvalidOperationException("Mã OTP đã hết hạn, vui lòng gửi lại OTP.");

            if (signer.OtpCode != otp)
            {
                signer.Status = "FAILED";
                await _signerRepo.UpdateAsync(signer);
                throw new InvalidOperationException("Mã OTP không chính xác.");
            }

            signer.Status = "VERIFIED";
            signer.IsSigned = true;
            signer.OtpVerifiedAt = DateTime.UtcNow;
            signer.SignedAt = DateTime.UtcNow;
            await _signerRepo.UpdateAsync(signer);

            var signerAccount = await _accountRepo.GetByIdAsync(signerUserId);
            var signerName = signerAccount?.FullName ?? "Thành viên";

            var vehicle = contract.Vehicle;
            var vehicleName = $"{vehicle.Make} {vehicle.Model}";
            var plate = string.IsNullOrWhiteSpace(vehicle.PlateNumber) ? "" : $" - {vehicle.PlateNumber}";

            await _noti.CreateAsync(
                contract.CreatedBy,
                "Thành viên đã ký hợp đồng",
                $"{signerName} đã ký hợp đồng xe {vehicleName}{plate}",
                "ECONTRACT_SIGNED",
                contract.Id
            );

            var members = contract.Signers.Select(s => s.UserId).Distinct().ToList();
            foreach (var uid in members.Where(uid => uid != signerUserId && uid != contract.CreatedBy))
            {
                await _noti.CreateAsync(
                    uid,
                    "Hợp đồng có cập nhật",
                    $"{signerName} đã ký hợp đồng xe {vehicleName}{plate}",
                    "ECONTRACT_SIGNED_UPDATE",
                    contract.Id
                );
            }

            bool allSigned = contract.Signers.All(s => s.IsSigned);
            if (allSigned)
            {
                contract.SignedAt = DateTime.UtcNow;
                contract.EffectiveFrom = contract.SignedAt;
                contract.Status = "PENDING_REVIEW";
                await _contractRepo.UpdateAsync(contract);
                foreach (var uid in members)
                {
                    await _noti.CreateAsync(
                        uid,
                        "Tất cả thành viên đã ký hợp đồng",
                        $"Hợp đồng xe {vehicleName}{plate} đã được ký đầy đủ và chờ duyệt.",
                        "ECONTRACT_ALL_SIGNED",
                        contract.Id
                    );
                }
            }
        }



        public async Task UpdateStatusAsync(Guid contractId, string newStatus)
        {
            var c = await _contractRepo.GetDetailAsync(contractId)
                    ?? throw new KeyNotFoundException("Contract not found");
            c.Status = newStatus;
            c.UpdatedAt = DateTime.UtcNow;
            await _contractRepo.SaveChangesAsync();
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

        public async Task<IEnumerable<ContractSummaryDto>> GetMyContractsAsync(Guid currentUserId)
        {

            var query = await _contractRepo.QueryAsync();

            var signerContractIds = await _signerRepo.GetContractIdsByUserAsync(currentUserId);


            query = query.Where(c => c.CreatedBy == currentUserId || signerContractIds.Contains(c.Id));

            var contracts = await query
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new ContractSummaryDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    TemplateName = c.Template.Name,
                    GroupName = c.Group.Name,
                    VehicleName = c.Vehicle.Make + " " + c.Vehicle.Model,
                    Status = c.Status,
                    CreatedAt = DateTimeHelper.ToVietnamTime(c.CreatedAt),
                    EffectiveFrom = c.EffectiveFrom,
                    ExpiresAt = c.ExpiresAt
                })
                .ToListAsync();

            return contracts;
        }

        private string GenerateOtpCode()
        {
            var secureChars = _config["MailSettings:SecureCharacters"] ?? "0123456789";
            var rng = RandomNumberGenerator.Create();
            var bytes = new byte[1];
            var codeBuilder = new StringBuilder();

            for (int i = 0; i < 6; i++)
            {
                rng.GetBytes(bytes);
                int index = bytes[0] % secureChars.Length;
                codeBuilder.Append(secureChars[index]);
            }
            return codeBuilder.ToString();
        }

        public async Task CancelContractAsync(Guid contractId, Guid currentUserId)
        {
            var contract = await _contractRepo.GetDetailAsync(contractId)
                          ?? throw new KeyNotFoundException("Không tìm thấy hợp đồng");

            if (contract.CreatedBy != currentUserId)
                throw new UnauthorizedAccessException("Bạn không có quyền hủy hợp đồng này.");

            if (contract.Status != ContractStatus.Draft)
                throw new InvalidOperationException("Chỉ có thể hủy hợp đồng ở trạng thái DRAFT.");

            contract.Status = ContractStatus.Canceled;
            contract.UpdatedAt = DateTime.UtcNow;

            await _contractRepo.SaveChangesAsync();
        }

public async Task ReviewContractAsync(Guid contractId, Guid staffId, bool approve, string? note)
    {

        var contract = await _contractRepo.GetDetailAsync(contractId)
                      ?? throw new KeyNotFoundException("Không tìm thấy hợp đồng.");

        if (contract.Status != ContractStatus.PendingReview)
            throw new InvalidOperationException("Chỉ có thể review hợp đồng ở trạng thái PENDING_REVIEW.");

        if (approve && (contract.Signers == null || contract.Signers.Any(s => !s.IsSigned)))
            throw new InvalidOperationException("Không thể phê duyệt vì vẫn còn người ký chưa hoàn tất.");
            
        var vehicle = contract.Vehicle!;
        var vehicleName = $"{vehicle.Make} {vehicle.Model}";
        var plate = string.IsNullOrWhiteSpace(vehicle.PlateNumber) ? "" : $" - {vehicle.PlateNumber}";
        var groupMembers = await _groupMemberRepo.GetByGroupIdAsync(contract.GroupId);

        contract.ReviewedAt = DateTime.UtcNow;
        contract.ReviewedBy = staffId;
        contract.ReviewNote = note ?? string.Empty;
        contract.UpdatedAt = DateTime.UtcNow;
        string renderedHtml = await GetPreviewHtmlAsync(contract.Id, staffId);
        contract.Content = renderedHtml;

        await _contractRepo.SaveChangesAsync();

            if (!approve)
            {
                contract.Status = ContractStatus.Rejected;
                await _contractRepo.SaveChangesAsync();

                await _noti.CreateAsync(
                    contract.CreatedBy,
                    "Hợp đồng bị từ chối ❌",
                    $"Hợp đồng sở hữu xe {vehicleName}{plate} đã bị từ chối.",
                    "ECONTRACT_REJECTED",
                    contract.Id
                );

                foreach (var m in groupMembers.Where(m => m.UserId != contract.CreatedBy))
                {
                    await _noti.CreateAsync(
                     m.UserId,
                     "Hợp đồng bị từ chối",
                     $"Hợp đồng xe {vehicleName}{plate} đã bị từ chối bởi quản trị viên.",
                     "ECONTRACT_REJECTED_UPDATE",
                     contract.Id
                    );
                }
                return;
            }

        if (string.IsNullOrWhiteSpace(contract.Content))
        throw new InvalidOperationException("Nội dung hợp đồng rỗng, không thể tạo PDF.");

        var pdfBytes = PdfHelper.RenderHtmlToPdf(contract.Content, $"Hợp đồng: {contract.Title}");

        string fileName = $"contract_{contract.Id}.pdf";
        string fileUrl = await _firebaseStorageService.UploadBytesAsync(pdfBytes, fileName, "contracts");

        contract.FileUrl = fileUrl;
        contract.Status = ContractStatus.Approved;
        if (contract.Vehicle != null)
            contract.Vehicle.Status = VehicleStatus.Active;

        await _contractRepo.SaveChangesAsync();

        foreach (var m in groupMembers)
        {
             await _noti.CreateAsync(
             m.UserId,
             "Hợp đồng được phê duyệt ✅",
             $"Hợp đồng sở hữu xe {vehicleName}{plate} đã được phê duyệt.",
             "ECONTRACT_APPROVED",
             contract.Id
                );
        }

        foreach (var signer in contract.Signers)
        {
            var user = signer.User;
            if (user?.Email is null) continue;

            var subject = $"Hợp đồng \"{contract.Title}\" đã được phê duyệt";
            var body = $@"
            <p>Xin chào {user.FullName},</p>
            <p>Hợp đồng <b>{contract.Title}</b> đã được phê duyệt và hoàn tất.</p>
            <p>Bạn có thể tải hợp đồng PDF tại liên kết sau:</p>
            <p><a href=""{fileUrl}"" target=""_blank"">📄 Tải hợp đồng (PDF)</a></p>
            <hr/>
            <p>Phê duyệt lúc: {DateTimeHelper.ToVietnamTime(DateTime.UtcNow):dd/MM/yyyy HH:mm:ss}</p>";

            await _mailService.SendEmailVerificationCode(user.Email, subject, body);
        }
    }


}
}
