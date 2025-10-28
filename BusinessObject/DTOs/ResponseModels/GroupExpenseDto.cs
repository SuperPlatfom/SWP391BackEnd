using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.ResponseModels
{
    public class GroupExpenseDto
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public Guid? ServiceRequestId { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime IncurredAt { get; set; }
        public string? ServiceType { get; set; }
    }
}
