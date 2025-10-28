using BusinessObject.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.RequestModels
{
    public class CreateServiceRequestRequest
    {
        public Guid GroupId { get; set; }
        public Guid VehicleId { get; set; }
        public Guid ServiceCenterId { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ServiceRequestType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
