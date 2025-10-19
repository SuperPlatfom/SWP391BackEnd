using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.ResponseModels
{
    public class ContractSummaryDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public string TemplateName { get; set; } = "";
        public string GroupName { get; set; } = "";
        public string VehicleName { get; set; } = "";
        public string Status { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
