using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.RequestModels
{
    public class CreateContractRequest
    {
        public Guid TemplateId { get; set; }
        public Guid GroupId { get; set; }
        public Guid VehicleId { get; set; }

        public DateTime? ExpiresAt { get; set; }
        public string? Title { get; set; }
        public List<OwnershipShareInput>? OwnershipShares { get; set; }
    }
    public class VerifyContractOtpRequest
    {
        public string Otp { get; set; } = string.Empty;
    }

    public class UpdateContractStatusRequest
    {
        public string NewStatus { get; set; } = string.Empty;
    }

    public class ReviewContractRequest
    {
        public bool Approve { get; set; }
        public string? Note { get; set; }
    }
}
