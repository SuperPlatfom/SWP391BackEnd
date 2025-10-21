using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    [Table("service_request_confirmation")]
    public class ServiceRequestConfirmation
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("request_id")]
        public Guid RequestId { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("decision")]
        public string Decision { get; set; } = "CONFIRM"; // CONFIRM/REJECT

        [Column("reason")]
        public string? Reason { get; set; }

        [Column("decided_at")]
        public DateTime DecidedAt { get; set; } = DateTime.UtcNow;

        public ServiceRequest Request { get; set; } = null!;
        public Account User { get; set; } = null!;
    }
}
