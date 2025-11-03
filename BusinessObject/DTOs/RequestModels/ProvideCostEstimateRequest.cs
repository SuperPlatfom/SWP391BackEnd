using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.RequestModels
{
    public class ProvideCostEstimateRequest
    {
        public decimal CostEstimate { get; set; }
        public DateTime? EstimatedFinishAt { get; set; }
        public string? Notes { get; set; }
    }
}
