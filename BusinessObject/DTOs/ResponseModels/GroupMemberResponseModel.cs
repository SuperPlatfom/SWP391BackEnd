

namespace BusinessObject.DTOs.ResponseModels
{
    public class GroupMemberResponseModel
    {
        public String InviteStatus { get; set; }

    public string RoleInGroup { get; set; }
        public string FullName { get; set; }
    }

    public class GroupDetailResponseModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string CreatedByName { get; set; } = null!;
        public bool IsActive { get; set; }

        public List<GroupMemberSimpleModel> Members { get; set; } = new();
    }

    public class GroupMemberSimpleModel
    {
        public string FullName { get; set; } = null!;
        public string RoleInGroup { get; set; } = null!;
    }
}
