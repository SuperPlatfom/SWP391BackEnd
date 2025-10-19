using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    [Table("e_contract")]
    public class EContract
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("group_id")]
        public Guid GroupId { get; set; }

        [Column("template_id")]
        public Guid TemplateId { get; set; }

        [Column("vehicle_id")]
        public Guid VehicleId { get; set; }

        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Column("content")]
        public string Content { get; set; } = string.Empty; // HTML content render từ template

        [Column("file_url")]
        public string? FileUrl { get; set; }

        [Column("status")]
        public string Status { get; set; } = "DRAFT"; // DRAFT / SIGNING / PENDING_REVIEW / APPROVED / REJECTED / ACTIVE / EXPIRED / VOID

        [Column("effective_from")]
        public DateTime? EffectiveFrom { get; set; }

        [Column("expires_at")]
        public DateTime? ExpiresAt { get; set; }

        [Column("signed_at")]
        public DateTime? SignedAt { get; set; }

        [Column("reviewed_at")]
        public DateTime? ReviewedAt { get; set; }

        [Column("reviewed_by")]
        public Guid? ReviewedBy { get; set; }

        [Column("review_note")]
        public string? ReviewNote { get; set; }

        [Column("created_by")]
        public Guid CreatedBy { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public CoOwnershipGroup Group { get; set; }
        public ContractTemplate Template { get; set; }
        public Vehicle Vehicle { get; set; }
        public Account CreatedByAccount { get; set; }
        public ICollection<EContractSigner> Signers { get; set; } = new List<EContractSigner>();
        public ICollection<EContractMemberShare> MemberShares { get; set; } = new List<EContractMemberShare>();

    }
}
