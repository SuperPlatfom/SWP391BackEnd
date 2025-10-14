using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    [Table("contract_variable")]
    public class ContractVariable
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("template_id")]
        public Guid TemplateId { get; set; }

        [Column("variable_name")]
        public string VariableName { get; set; }

        [Column("display_label")]
        public string DisplayLabel { get; set; }

        [Column("input_type")]
        public string InputType { get; set; } // text, number, date, textarea, group

        [Column("is_required")]
        public bool IsRequired { get; set; } = false;

        [Column("default_value")]
        public string? DefaultValue { get; set; }

        public ContractTemplate Template { get; set; }
    }
}
