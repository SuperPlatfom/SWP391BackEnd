using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.ResponseModels
{
    public class ContractSignerDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = "";
        public string? Email { get; set; }
        public string Status { get; set; } = "PENDING";
        public DateTime? OtpSentAt { get; set; }
        public DateTime? OtpVerifiedAt { get; set; }
    }
}
