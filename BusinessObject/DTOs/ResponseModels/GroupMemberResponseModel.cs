

namespace BusinessObject.DTOs.ResponseModels
{
    public class GroupMemberResponseModel
    {
        public Guid UserId { get; set; }
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public String InviteStatus { get; set; }

    public string RoleInGroup { get; set; }
        public string FullName { get; set; }
        public string GroupName { get; set; }
    }
}
