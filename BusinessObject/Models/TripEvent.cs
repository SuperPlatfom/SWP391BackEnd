
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
        public string EventType { get; set; } = "CHECKIN";
        // CHECKIN / CHECKOUT / DAMAGE / NOTE

        [Column("signed_by")]
        [ForeignKey(nameof(SignedByUser))]
        public Guid SignedBy { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("photos_url", TypeName = "text")]
        public string? PhotosUrl { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        //  Navigation Properties
        public Account SignedByUser { get; set; } = null!;
    }
}
