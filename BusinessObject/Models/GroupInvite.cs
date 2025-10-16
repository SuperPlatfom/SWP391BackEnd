using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    [Table("group_invite")]
    public class GroupInvite
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("group_id")]
        [ForeignKey(nameof(Group))]
        public Guid GroupId { get; set; }
        public CoOwnershipGroup Group { get; set; } = null!;

        [Column("invite_code")]
        public string InviteCode { get; set; } = null!;

        [Column("expires_at")]
        public DateTime ExpiresAt { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}