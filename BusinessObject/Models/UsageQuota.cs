

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
        [Column("vehicle_id")]
        [ForeignKey(nameof(Vehicle))]
        public Guid VehicleId { get; set; }
        public Vehicle Vehicle { get; set; } = null!;

        [Required]
        [Column("week_start_date")]
        public DateTime WeekStartDate { get; set; }

        [Required]
        [Column("hours_limit")]
        public decimal HoursLimit { get; set; }

        [Column("hours_used")]
        public decimal HoursUsed { get; set; } = 0;
        [Column("hours_debt")]
        public decimal HoursDebt { get; set; } = 0;
        [Column("hours_advance")]
        public decimal HoursAdvance { get; set; } = 0;

        [Column("last_updated")]
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
