using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    [Table("contract_clause")]
    public class ContractClause
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("template_id")]
        public Guid TemplateId { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("body")]
        public string Body { get; set; } 

        [Column("order_index")]
        public int OrderIndex { get; set; }

        [Column("is_mandatory")]
        public bool IsMandatory { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ContractTemplate Template { get; set; }
    }
}
