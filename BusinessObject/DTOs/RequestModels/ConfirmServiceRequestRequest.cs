using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.RequestModels
{
    public class ConfirmServiceRequestRequest
    {
        public Guid RequestId { get; set; }
        public bool Confirm { get; set; }
        public string? Reason { get; set; }
    }
}
