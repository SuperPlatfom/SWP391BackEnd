


using BusinessObject.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BusinessObject.Models
{
    [Table("booking")]
    public class Booking
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("group_id")]
        [ForeignKey(nameof(Group))]
        public Guid GroupId { get; set; }

        [Column("vehicle_id")]
        [ForeignKey(nameof(Vehicle))]
        public Guid VehicleId { get; set; }

        [Column("user_id")]
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        [Column("start_time")]
        public DateTime StartTime { get; set; }

        [Column("end_time")]
        public DateTime EndTime { get; set; }

        [Column("purpose")]
        public string? Purpose { get; set; }

        [Column("status")]
        [MaxLength(20)]
        public string Status { get; set; } = BookingStatus.Booked;
        // BOOKED / IN_USE / OVERTIME / COMPLETED / CANCELED

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        //  Navigation Properties
        public Account User { get; set; } = null!;
        public CoOwnershipGroup Group { get; set; } = null!;
        public Vehicle Vehicle { get; set; } = null!;

        public UsageSession? UsageSession { get; set; }

    }
}
