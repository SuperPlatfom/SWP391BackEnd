using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.RequestModels
{
    public class ServiceRequestConfirmationDto
    {
        public Guid Id { get; set; }
        public Guid RequestId { get; set; }
        public Guid UserId { get; set; }
        public string Decision { get; set; } = string.Empty; 
        public string? Reason { get; set; }
        public DateTime DecidedAt { get; set; }
    }
}
