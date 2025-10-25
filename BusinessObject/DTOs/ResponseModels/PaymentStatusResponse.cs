using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.ResponseModels
{
    public class PaymentStatusResponse
    {
        public string OrderCode { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;   // PENDING/PAID/FAILED/CANCELED
        public string? PaidAt { get; set; }                  
        public string Title { get; set; } = string.Empty;    
    }
    public class PaymentHistoryItem
    {
        public string OrderCode { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? PaidAt { get; set; }
        public string Method { get; set; } = "PayOs";
        public string Description { get; set; } = string.Empty; 
    }
}
