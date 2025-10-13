

namespace BusinessObject.DTOs.ResponseModels
{
   public class GroupResponseModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public Guid CreatedBy { get; set; } 
        public bool IsActive { get; set; }
        public string? GovernancePolicy { get; set; }
        public List<GroupMemberResponseModel> Members { get; set; } = new();
        public List<VehicleResponseModel> Vehicles { get; set; } = new();
    }
}
