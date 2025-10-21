

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    [Table("usage_session")]
    public class UsageSession
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("booking_id")]
        [ForeignKey(nameof(Booking))]
        public Guid BookingId { get; set; }

        [Column("user_id")]
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        [Column("group_id")]
        [ForeignKey(nameof(Group))]
        public Guid GroupId { get; set; }

        [Column("vehicle_id")]
        [ForeignKey(nameof(Vehicle))]
        public Guid VehicleId { get; set; }

        [Column("start_time")]
        public DateTime StartTime { get; set; }

        [Column("end_time")]
        public DateTime? EndTime { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        [Column("status")]
        [MaxLength(20)]
        public string Status { get; set; } = "ONGOING";
        // ONGOING / COMPLETED / CLOSED

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // 🔗 Navigation Properties
        public Booking Booking { get; set; } = null!;
        public Account User { get; set; } = null!;
        public CoOwnershipGroup Group { get; set; } = null!;
        public Vehicle Vehicle { get; set; } = null!;

        public ICollection<TripEvent> TripEvents { get; set; } = new List<TripEvent>();
    }
}
