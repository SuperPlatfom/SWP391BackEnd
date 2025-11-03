using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.RequestModels
{
    public class AttachVehicleRequest
    {
        public Guid GroupId { get; set; }
        public Guid VehicleId { get; set; }
    }

}
