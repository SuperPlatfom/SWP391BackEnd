using System;
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

        [Column("governance_policy")]
        public string? GovernancePolicy { get; set; } // JSON: quy tắc biểu quyết, quorum, chia phí,...

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Quan hệ 1-N với group_member
        public ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();

        // Quan hệ 1-N với Vehicle
        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        
    }
}
