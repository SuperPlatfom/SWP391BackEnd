
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    [Table("trip_event")]
    public class TripEvent
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }



        [Column("event_type")]
        [MaxLength(20)]
        public string EventType { get; set; }
        // CHECKIN / CHECKOUT / DAMAGE 

        [Column("signed_by")]
        [ForeignKey(nameof(SignedByUser))]
        public Guid SignedBy { get; set; }

        [Column("vehicle_id")]
        [ForeignKey(nameof(Vehicle))]
        public Guid VehicleId { get; set; }

        [Column("booking_id")]
        public Guid? BookingId { get; set; }

       

        [Column("description")]
        public string? Description { get; set; } = string.Empty;

        [Column("photos_url", TypeName = "text")]
        public string? PhotosUrl { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        //  Navigation Properties
        public Account SignedByUser { get; set; } = null!;
        public Vehicle Vehicle { get; set; }
        public Booking? Booking { get; set; }
    }
}
