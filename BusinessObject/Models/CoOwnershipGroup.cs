
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    [Table("co_ownership_group")]
    public class CoOwnershipGroup
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("name")]
        [Required]
        public string Name { get; set; } = string.Empty;

        

        [Column("created_by")]
        [ForeignKey(nameof(CreatedByAccount))]
        public Guid CreatedBy { get; set; }

        public Account CreatedByAccount { get; set; } = null!;


        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    
        // Quan hệ 1-N với group_member
        public ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();
        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public ICollection<GroupInvite> Invites { get; set; } = new List<GroupInvite>();
        public ICollection<EContract> Contracts { get; set; } = new List<EContract>();
        public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
        public ICollection<GroupExpense> GroupExpenses { get; set; } = new List<GroupExpense>();
        public ICollection<MemberInvoice> MemberInvoices { get; set; } = new List<MemberInvoice>();



        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        public ICollection<UsageQuota> usageQuotas { get; set; } = new List<UsageQuota>();
    }
}
