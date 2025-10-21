

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    [Table("usage_quota")]
    public class UsageQuota
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("account_id")]
        [ForeignKey(nameof(Account))]
        public Guid AccountId { get; set; }
        public Account Account { get; set; } = null!;

        [Required]
        [Column("group_id")]
        [ForeignKey(nameof(Group))]
        public Guid GroupId { get; set; }
        public CoOwnershipGroup Group { get; set; } = null!;

        [Required]
        [Column("week_start_date")]
        public DateTime WeekStartDate { get; set; }

        [Required]
        [Column("hours_limit")]
        public double HoursLimit { get; set; }

        [Column("hours_used")]
        public double HoursUsed { get; set; } = 0;

        [Column("last_updated")]
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
