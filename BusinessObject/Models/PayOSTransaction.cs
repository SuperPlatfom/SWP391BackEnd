using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    [Table("payos_transaction")]
    public class PayOSTransaction
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("payment_id")]
        public Guid PaymentId { get; set; }

        [Column("order_code")]
        public string OrderCode { get; set; } = string.Empty;

        [Column("qr_code_url")]
        public string QrCodeUrl { get; set; } = string.Empty;

        [Column("deeplink_url")]
        public string DeeplinkUrl { get; set; } = string.Empty;

        [Column("expired_at")]
        public DateTime ExpiredAt { get; set; }

        [Column("status")]
        public string Status { get; set; } = "INIT";

        [Column("webhook_received_at")]
        public DateTime? WebhookReceivedAt { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Payment Payment { get; set; } = null!;
    }
}
