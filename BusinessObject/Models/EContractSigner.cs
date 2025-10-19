using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    [Table("e_contract_signer")]
    public class EContractSigner
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("contract_id")]
        public Guid ContractId { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("otp_code")]
        public string? OtpCode { get; set; }
        [Column("code_expiry")]
        public DateTime? CodeExpiry { get; set; }

        [Column("otp_sent_at")]
        public DateTime? OtpSentAt { get; set; }

        [Column("otp_verified_at")]
        public DateTime? OtpVerifiedAt { get; set; }

        [Column("is_signed")]
        public bool IsSigned { get; set; } = false;

        [Column("status")]
        public string Status { get; set; } = "PENDING"; // PENDING / VERIFIED / FAILED

        [Column("signed_at")]
        public DateTime? SignedAt { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public EContract Contract { get; set; }
        public Account User { get; set; }
    }
}
