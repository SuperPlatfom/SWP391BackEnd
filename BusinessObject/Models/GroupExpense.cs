using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    [Table("group_expense")]
    public class GroupExpense
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("group_id")]
        public Guid GroupId { get; set; }

        [Column("category_id")]
        public int CategoryId { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("amount", TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Column("incurred_at")]
        public DateTime IncurredAt { get; set; } = DateTime.UtcNow;

        [Column("invoice_url")]
        public string? InvoiceUrl { get; set; }

        [Column("status")]
        public string Status { get; set; } = "DRAFT";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public CoOwnershipGroup Group { get; set; } = null!;
        public ExpenseCategory Category { get; set; } = null!;
        public ICollection<MemberInvoice> MemberInvoices { get; set; } = new List<MemberInvoice>();
        public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
    }
}
