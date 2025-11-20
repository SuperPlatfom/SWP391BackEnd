using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.ResponseModels
{
    public class RevenueStatisticResponse
    {
    
        public decimal? TotalRevenue { get; set; }
        public int? CompletedOrders { get; set; }
        public int? VehiclesServiced { get; set; }
        public List<TechnicianRevenueModel?> TechnicianRevenue { get; set; }
    }

    public class TechnicianRevenueModel
    {
        public string TechnicianName { get; set; }
        public decimal? Revenue { get; set; }
    }
}
