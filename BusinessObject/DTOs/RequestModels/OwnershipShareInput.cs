using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.RequestModels
{
    public class OwnershipShareInput
    {
        public Guid UserId { get; set; }
        public decimal Rate { get; set; } 
    }
}
