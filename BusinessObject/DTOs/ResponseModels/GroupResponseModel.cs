

namespace BusinessObject.DTOs.ResponseModels
{
   public class GroupResponseModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string CreatedByName { get; set; } = null!;
        public bool IsActive { get; set; }

        public List<GroupMemberResponseModel> Members { get; set; } = new();
        public List<VehicleResponseModel> Vehicles { get; set; } = new();
    }

    public class BasicGroupReponseModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CreatedByName { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    public class GroupInviteResponseModel
    {
        public string InviteCode { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
    }


}
