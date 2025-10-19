using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    [Table("econtract_member_share")]
    public class EContractMemberShare
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("contract_id")]
        public Guid ContractId { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("ownership_rate", TypeName = "decimal(5,2)")]
        public decimal? OwnershipRate { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public EContract Contract { get; set; } = null!;
        public Account User { get; set; } = null!;
    }
}
