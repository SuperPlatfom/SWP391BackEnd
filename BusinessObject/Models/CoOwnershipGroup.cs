
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    [Table("co_ownership_group")]
    public class CoOwnershipGroup
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("name")]
        [Required]
        public string Name { get; set; } = string.Empty;

        

        [Column("created_by")]
        [ForeignKey(nameof(CreatedByAccount))]
        public Guid CreatedBy { get; set; }

        public Account CreatedByAccount { get; set; } = null!;


        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Column("weekly_quota_hours")]
        public double WeeklyQuotaHours { get; set; } = 20;
        // Quan hệ 1-N với group_member
        public ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();

        // Quan hệ 1-N với Vehicle
        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

        // Quan hệ 1-N với group_invite
        public ICollection<GroupInvite> Invites { get; set; } = new List<GroupInvite>();
        public ICollection<EContract> Contracts { get; set; } = new List<EContract>();


        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<UsageSession> UsageSessions { get; set; } = new List<UsageSession>();
        public ICollection<UsageQuota> usageQuotas { get; set; } = new List<UsageQuota>();
    }
}
