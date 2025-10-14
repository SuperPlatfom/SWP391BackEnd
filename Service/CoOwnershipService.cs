using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;

using Repository.Interfaces;
using Service.Interfaces;
using System.Security.Principal;

namespace Service
{
    public class CoOwnershipService : ICoOwnershipService
    {
        private readonly ICoOwnershipGroupRepository _repository;
        private readonly IGroupMemberRepository _memberRepository;
        public CoOwnershipService(ICoOwnershipGroupRepository repository, IGroupMemberRepository memberRepository)
        {
            _repository = repository;
            _memberRepository = memberRepository;

        }

        public async Task<GroupResponseModel> GetGroupByIdAsync(Guid groupId)
        {
            var group = await _repository.GetByIdAsync(groupId);
            if (group == null)
                throw new KeyNotFoundException($"Không tìm thấy nhóm với ID {groupId}.");

            return new GroupResponseModel
            {
                Id = group.Id,
                Name = group.Name,
                GovernancePolicy = group.GovernancePolicy,
                IsActive = group.IsActive,
                Members = group.Members?.Select(m => new GroupMemberResponseModel
                {
                    Id = m.Id,
                    GroupId = m.GroupId,
                    UserId = m.UserId,
                    RoleInGroup = m.RoleInGroup,
                    InviteStatus = m.InviteStatus,
                    FullName = m.UserAccount?.FullName ?? "",
                    GroupName = m.Group?.Name ?? ""
                }).ToList()
            };
        }
        public async Task<GroupResponseModel> UpdateGroupAsync(Guid groupId, string newName, string? newGovernancePolicy)
        {
            var group = await _repository.GetByIdAsync(groupId);
            if (group == null)
                throw new KeyNotFoundException("Không tìm thấy nhóm để cập nhật.");

            // Chỉ cập nhật các trường trong CoOwnershipGroup
            group.Name = newName;
            if (!string.IsNullOrWhiteSpace(newGovernancePolicy))
                group.GovernancePolicy = newGovernancePolicy;

            group.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateGroupAsync(group);

            // Trả về DTO chỉ chứa CoOwnershipGroup
            return new GroupResponseModel
            {
                Id = group.Id,
                Name = group.Name,
                GovernancePolicy = group.GovernancePolicy,
                IsActive = group.IsActive
            };
        }

        public async Task<GroupResponseModel> CreateGroupAsync(CreateGroupRequest request)
        {
            // ✅ Kiểm tra account tồn tại
            var accountExists = await _repository.AccountExistsAsync(request.CreatedBy);
            if (!accountExists)
                throw new KeyNotFoundException($"Account ID {request.CreatedBy} không tồn tại.");

            // Tạo nhóm mới
            var group = new CoOwnershipGroup
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                CreatedBy = request.CreatedBy,
                GovernancePolicy = request.GovernancePolicy,
                IsActive= true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            using var transaction = await _repository.BeginTransactionAsync();

            try
            {
                // Thêm group vào DB
                await _repository.AddGroupAsync(group);


                Vehicle? vehicle = null;
                // Nếu có VehicleId, gán về nhóm
                if (request.VehicleId != null)
                {
                     vehicle = await _repository.GetVehicleByIdAsync(request.VehicleId.Value);
                    if (vehicle != null)
                    {
                        vehicle.GroupId = group.Id;
                        await _repository.UpdateVehicleAsync(vehicle);
                    }
                }

                // Thêm OWNER vào group
                var ownerMember = new GroupMember
                {
                    Id = Guid.NewGuid(),
                    GroupId = group.Id,
                    UserId = request.CreatedBy,
                    RoleInGroup = "OWNER",
                    InviteStatus = "ACCEPTED",
                    JoinDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true
                };
                await _memberRepository.AddGroupMemberAsync(ownerMember);

                await transaction.CommitAsync();

                // --- Build ResponseModel ---
                var account = await _repository.GetAccountByIdAsync(request.CreatedBy);
                var response = new GroupResponseModel
                {
                    Id = group.Id,
                    Name = group.Name,
                    CreatedBy = group.CreatedBy,
                    GovernancePolicy = group.GovernancePolicy,
                   
                    Members = new List<GroupMemberResponseModel>
            {
                new GroupMemberResponseModel
                {
                    Id = ownerMember.Id,
                    GroupId = ownerMember.GroupId,
                    UserId = request.CreatedBy,
                    RoleInGroup = "OWNER",
                    FullName = account?.FullName ?? ""
                }
            },
                    Vehicles = request.VehicleId != null
                        ? new List<VehicleResponseModel>
                        {
                    new VehicleResponseModel
                    {
                        Id = request.VehicleId.Value,
                        PlateNumber = vehicle?.PlateNumber,
                        Make = vehicle?.Make,
                        Model = vehicle?.Model,
                        ModelYear = vehicle?.ModelYear ?? 0,
                        Color = vehicle?.Color,
                        BatteryCapacityKwh = vehicle?.BatteryCapacityKwh ?? 0,
                        RangeKm = vehicle?.RangeKm ?? 0,
                        TelematicsDeviceId = vehicle?.TelematicsDeviceId,
                        Status = vehicle?.Status
                    }
                        }
                        : new List<VehicleResponseModel>()
                };

                return response;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<GroupMember> InviteMemberAsync(InviteRequest request)
        {
            var exists = await _memberRepository.GroupMemberExistsAsync(request.GroupId, request.UserId);
            if (exists)
                throw new InvalidOperationException("User đã được mời hoặc đã tham gia nhóm.");

            var member = new GroupMember
            {
                Id = Guid.NewGuid(),
                GroupId = request.GroupId,
                UserId = request.UserId,
                RoleInGroup = "MEMBER",
                InviteStatus = "PENDING",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _memberRepository.AddGroupMemberAsync(member);
            return member;
        }

        

        public async Task<List<GroupBasicReponseModel>> GetGroupsByUserAsync(Guid userId)
        {
            var groups = await _repository.GetGroupsByUserAsync(userId);

            return groups.Select(g => new GroupBasicReponseModel
            {
                Id = g.Id,
                Name = g.Name,
                CreatedBy = g.CreatedBy,
                GovernancePolicy = g.GovernancePolicy,
                IsActive = g.IsActive
              
            }).ToList();
        }

       

        public async Task<List<GroupResponseModel>> GetAllGroupsAsync()
        {
            var groups = await _repository.GetAllGroupsAsync();

            return groups.Select(g => new GroupResponseModel
            {
                Id = g.Id,
                Name = g.Name,
                CreatedBy = g.CreatedBy,
                IsActive = g.IsActive,
                Members = g.Members.Select(m => new GroupMemberResponseModel
                {
                    Id = m.Id,
                    GroupId = m.GroupId,
                    UserId = m.UserId,
                    RoleInGroup = m.RoleInGroup,
                    InviteStatus = m.InviteStatus,
                    FullName = m.UserAccount?.FullName ?? ""
                }).ToList(),
                Vehicles = g.Vehicles.Select(v => new VehicleResponseModel 
                {
                    Id = v.Id,
                    PlateNumber = v.PlateNumber,
                    Make = v.Make,
                    Model = v.Model,
                    ModelYear = v.ModelYear,
                    BatteryCapacityKwh = v.BatteryCapacityKwh,
                    RangeKm = v.RangeKm,
                    TelematicsDeviceId = v.TelematicsDeviceId,
                    Status = v.Status
                }).ToList()
            }).ToList();
        }

        
    }
}
