using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    [Table("payment")]
    public class Payment
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("invoice_id")]
        public Guid? InvoiceId { get; set; }

        [Column("amount", TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Column("payment_method")]
        public string PaymentMethod { get; set; } = "PayOS";

        [Column("status")]
        public string Status { get; set; } = "PENDING";

        [Column("transaction_code")]
        public string TransactionCode { get; set; } = string.Empty;

        [Column("paid_at")]
        public DateTime? PaidAt { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Account User { get; set; } = null!;
        public MemberInvoice? Invoice { get; set; }
        public PayOSTransaction? PayOSTransaction { get; set; }
    }
}
