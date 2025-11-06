using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using Microsoft.AspNetCore.Http;
using Repository.Interfaces;
using Service.Interfaces;
using System.Security.Claims;
using System.Security.Principal;

namespace Service
{
    public class CoOwnershipService : ICoOwnershipService
    {
        private readonly ICoOwnershipGroupRepository _repository;
        private readonly IGroupMemberRepository _memberRepository;
        private readonly IVehicleRepository _vehicleRepository;
        public CoOwnershipService(ICoOwnershipGroupRepository repository, IGroupMemberRepository memberRepository, IVehicleRepository vehicleRepository )
        {
            _repository = repository;
            _memberRepository = memberRepository;
            _vehicleRepository = vehicleRepository;

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
                CreatedByName = group.CreatedByAccount?.FullName ?? "",
                IsActive = group.IsActive,

                Members = group.Members?.Select(m => new GroupMemberResponseModel
                {                             
                    UserId = m.UserId,
                    FullName = m.UserAccount?.FullName ?? "",
                    RoleInGroup = m.RoleInGroup,
                    InviteStatus = m.InviteStatus
                }).ToList() ?? new List<GroupMemberResponseModel>(),

                // Danh sách vehicle
                Vehicles = group.Vehicles?.Select(v => new VehicleResponseModel
                {
                    Id = v.Id,
                    PlateNumber = v.PlateNumber,
                    Make = v.Make,
                    Model = v.Model,
                    ModelYear = v.ModelYear,
                    Color = v.Color,
                    Status = v.Status,
                    BatteryCapacityKwh = v.BatteryCapacityKwh,
                    RangeKm = v.RangeKm,
                    GroupId = v.GroupId
                }).ToList() ?? new List<VehicleResponseModel>()
            };
        }
        public async Task<GroupResponseModel> UpdateGroupAsync(Guid groupId, string newName)
        {
            // 1️ Tìm group theo ID
            var group = await _repository.GetByIdAsync(groupId);
            if (group == null)
                throw new KeyNotFoundException("Không tìm thấy nhóm để cập nhật.");

            // 2️ Kiểm tra tên hợp lệ
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Tên nhóm không được để trống.");



            // 3️ Cập nhật duy nhất trường Name
            group.Name = newName.Trim();
            group.UpdatedAt = DateTime.UtcNow;

            // 4️ Lưu thay đổi
            await _repository.UpdateGroupAsync(group);

            // 5️⃣ Trả về model phản hồi chỉ chứa thông tin cơ bản
            return new GroupResponseModel
            {
                Id = group.Id,
                Name = group.Name,
                IsActive = group.IsActive
            };
        }
        public async Task<List<VehicleResponseModel>> GetVehiclesByGroupIdAsync(Guid groupId)
        {
            var group = await _repository.GetByIdAsync(groupId);
            if (group == null)
                throw new KeyNotFoundException($"Không tìm thấy nhóm với ID {groupId}.");

            return group.Vehicles?
                .Select(v => new VehicleResponseModel
                {
                    Id = v.Id,
                    PlateNumber = v.PlateNumber,
                    Make = v.Make,
                    Model = v.Model,
                    ModelYear = v.ModelYear,
                    Color = v.Color,
                    Status = v.Status,
                    BatteryCapacityKwh = v.BatteryCapacityKwh,
                    RangeKm = v.RangeKm,
                    VehicleImageUrl = v.VehicleImageUrl,
                    GroupId = v.GroupId
                }).ToList() ?? new List<VehicleResponseModel>();
        }

        public async Task<GroupResponseModel> CreateGroupAsync(CreateGroupRequest request, Guid userId)
        {
            // ✅ Kiểm tra account tồn tại
            var accountExists = await _repository.AccountExistsAsync(userId);
            if (!accountExists)
                throw new KeyNotFoundException($"Account ID {userId} không tồn tại.");

            // ✅ Tạo group mới
            var group = new CoOwnershipGroup
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                CreatedBy = userId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            using var transaction = await _repository.BeginTransactionAsync();

            try
            {
                await _repository.AddGroupAsync(group);

                // ✅ Thêm OWNER vào group
                var ownerMember = new GroupMember
                {
                    Id = Guid.NewGuid(),
                    GroupId = group.Id,
                    UserId = userId,
                    RoleInGroup = "OWNER",
                    InviteStatus = "ACCEPTED",
                    JoinDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true
                };
                await _memberRepository.AddGroupMemberAsync(ownerMember);

                await transaction.CommitAsync();

                var account = await _repository.GetAccountByIdAsync(userId);

                // ✅ Trả về dữ liệu nhóm
                var response = new GroupResponseModel
                {
                    Id = group.Id,
                    Name = group.Name,
                    IsActive = group.IsActive,
                    Members = new List<GroupMemberResponseModel>
            {
                new GroupMemberResponseModel
                {


                    RoleInGroup = "OWNER",
                    FullName = account?.FullName ?? ""
                }
            },
                    Vehicles = new List<VehicleResponseModel>() // mặc định rỗng
                };

                return response;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public async Task AttachVehicleToGroupAsync(Guid userId, Guid groupId, Guid vehicleId)
        {
            // 🔹 Kiểm tra group có tồn tại
            var group = await _repository.GetGroupByIdAsync(groupId);
            if (group == null)
                throw new KeyNotFoundException("Nhóm không tồn tại.");

            // 🔹 Kiểm tra user có phải OWNER trong group không
            var member = await _memberRepository.GetGroupMemberAsync(groupId, userId);
            if (member == null || member.RoleInGroup != "OWNER")
                throw new UnauthorizedAccessException("Chỉ chủ nhóm (OWNER) mới được phép gắn xe vào nhóm.");

            // 🔹 Kiểm tra vehicle tồn tại
            var vehicle = await _repository.GetVehicleByIdAsync(vehicleId);
            if (vehicle == null)
                throw new KeyNotFoundException("Xe không tồn tại.");

            // 🔹 Kiểm tra xe đã thuộc nhóm khác chưa
            if (vehicle.GroupId != null && vehicle.GroupId != groupId)
                throw new InvalidOperationException("Xe này đã thuộc một nhóm khác.");

            // 🔹 Gán xe vào nhóm
            vehicle.GroupId = groupId;
            await _repository.UpdateVehicleAsync(vehicle);
        }

        public async Task DetachVehicleFromGroupAsync(Guid userId, Guid groupId, Guid vehicleId)
        {
            // 🔹 Kiểm tra group có tồn tại
            var group = await _repository.GetGroupByIdAsync(groupId);
            if (group == null)
                throw new KeyNotFoundException("Nhóm không tồn tại.");

            // 🔹 Kiểm tra user có phải OWNER trong group không
            var member = await _memberRepository.GetGroupMemberAsync(groupId, userId);
            if (member == null || member.RoleInGroup != "OWNER")
                throw new UnauthorizedAccessException("Chỉ chủ nhóm (OWNER) mới được phép gỡ xe khỏi nhóm.");

            // 🔹 Kiểm tra vehicle
            var vehicle = await _repository.GetVehicleByIdAsync(vehicleId);
            if (vehicle == null)
                throw new KeyNotFoundException("Xe không tồn tại.");

            // 🔹 Kiểm tra xe có thuộc nhóm hiện tại không
            if (vehicle.GroupId != groupId)
                throw new InvalidOperationException("Xe này không thuộc nhóm này.");

            // 🔹 Gỡ liên kết
            vehicle.GroupId = null;
            await _repository.UpdateVehicleAsync(vehicle);
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



        public async Task<List<BasicGroupReponseModel>> GetGroupsByCurrentUserAsync(ClaimsPrincipal user)
{
    if (user == null || !user.Identity?.IsAuthenticated == true)
        throw new UnauthorizedAccessException("Bạn cần đăng nhập.");

    var userIdStr = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(userIdStr))
        throw new UnauthorizedAccessException("Không tìm thấy UserId.");

    var userId = Guid.Parse(userIdStr);

    var groups = await _repository.GetGroupsByUserAsync(userId);

    var result = new List<BasicGroupReponseModel>();
    foreach (var g in groups)
    {
        var owner = await _repository.GetAccountByIdAsync(g.CreatedBy);
        result.Add(new BasicGroupReponseModel
        {
            Id = g.Id,
            Name = g.Name,
           
            CreatedByName = owner?.FullName ?? "Unknown",
            IsActive = g.IsActive
        });
    }

    return result;
}



        public async Task<List<GroupResponseModel>> GetAllGroupsAsync()
        {
            var groups = await _repository.GetAllGroupsAsync();

            return groups.Select(g => new GroupResponseModel
            {
                Id = g.Id,
                Name = g.Name,
                CreatedByName = g.CreatedByAccount?.FullName ?? "",
                IsActive = g.IsActive,
                Members = g.Members.Select(m => new GroupMemberResponseModel
                {
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
                    Status = v.Status
                }).ToList()
            }).ToList();
        }

        public async Task<bool> ActivateVehicleAsync(Guid vehicleId, Guid userId)
        {
            var vehicle = await _repository.GetVehicleByIdAsync(vehicleId)
                          ?? throw new KeyNotFoundException("Không tìm thấy xe.");

            if (vehicle.GroupId == null)
                throw new InvalidOperationException("Xe này chưa thuộc nhóm nào.");

            // Kiểm tra user có trong nhóm và là OWNER không
            var member = await _memberRepository.GetByUserAndGroupAsync(userId, vehicle.GroupId.Value);
            if (member == null || member.RoleInGroup != "OWNER")
                throw new UnauthorizedAccessException("Chỉ chủ nhóm mới có quyền kích hoạt xe.");

            vehicle.Status = "ACTIVE";
            vehicle.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateVehicleAsync(vehicle);

            return true;
        }

        public async Task<bool> DeactivateVehicleAsync(Guid vehicleId, Guid userId)
        {
            var vehicle = await _repository.GetVehicleByIdAsync(vehicleId)
                          ?? throw new KeyNotFoundException("Không tìm thấy xe.");

            if (vehicle.GroupId == null)
                throw new InvalidOperationException("Xe này chưa thuộc nhóm nào.");

            // Kiểm tra user có trong nhóm và là OWNER không
            var member = await _memberRepository.GetByUserAndGroupAsync(userId, vehicle.GroupId.Value);
            if (member == null || member.RoleInGroup != "OWNER")
                throw new UnauthorizedAccessException("Chỉ chủ nhóm mới có quyền hủy kích hoạt xe.");

            vehicle.Status = "INACTIVE";
            vehicle.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateVehicleAsync(vehicle);

            return true;
        }
    }
}
