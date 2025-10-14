
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    [Table("group_member")]
    public class GroupMember
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("group_id")]
        [ForeignKey(nameof(Group))]
        public Guid GroupId { get; set; }
        public CoOwnershipGroup Group { get; set; } = null!;

        [Column("user_id")]
        [ForeignKey(nameof(UserAccount))]
        public Guid UserId { get; set; }
        public Account UserAccount { get; set; } = null!; 

        [Column("role_in_group")]
        [Required]
        public string RoleInGroup { get; set; } = "MEMBER";

        [Column("join_date")]
        public DateTime JoinDate { get; set; } = DateTime.UtcNow;

        [Column("invite_status")]
        [Required]
        public string InviteStatus { get; set; } = "PENDING";

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
