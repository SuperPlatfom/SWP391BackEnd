using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    [Table("member_invoice")]
    public class MemberInvoice
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("expense_id")]
        public Guid ExpenseId { get; set; }

        [Column("group_id")]
        public Guid GroupId { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("total_amount", TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Column("amount_paid", TypeName = "decimal(18,2)")]
        public decimal AmountPaid { get; set; }

        [Column("status")]
        public string Status { get; set; } = "DUE";

        [Column("due_date")]
        public DateTime? DueDate { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public GroupExpense Expense { get; set; } = null!;
        public CoOwnershipGroup Group { get; set; } = null!;
        public Account User { get; set; } = null!;
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
