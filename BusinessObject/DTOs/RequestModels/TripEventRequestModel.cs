using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.RequestModels
{
    public class TripEventRequestModel
    {
        [JsonPropertyName("BookingId")]
        public Guid BookingId { get; set; }

        public string? Description { get; set; }

        [JsonPropertyName("Photo")]
        public IFormFile? Photo { get; set; } 
    }
}
