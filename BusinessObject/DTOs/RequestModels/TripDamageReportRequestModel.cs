

using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace BusinessObject.DTOs.RequestModels
{
    public class TripDamageReportRequestModel
    {
        [JsonPropertyName("Id")]
        public Guid Id { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("photo")]
        public IFormFile? Photo { get; set; }
    }
}
