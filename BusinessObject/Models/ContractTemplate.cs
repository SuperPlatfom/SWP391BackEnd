using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    [Table("contract_template")]
    public class ContractTemplate
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("name")]
        [Required]
        public string Name { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("version")]
        public string Version { get; set; }

        [Column("content")]
        public string Content { get; set; } // HTML Template (chứa {{placeholder}})

        [Column("min_coowners")]
        public int MinCoOwners { get; set; } = 2;

        [Column("max_coowners")]
        public int MaxCoOwners { get; set; } = 5;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<ContractClause> Clauses { get; set; } = new List<ContractClause>();
        public ICollection<ContractVariable> Variables { get; set; } = new List<ContractVariable>();
    }
}
