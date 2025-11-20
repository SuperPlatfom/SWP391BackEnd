

namespace BusinessObject.DTOs.ResponseModels
{
    public class TripDamageReportResponse
    {
      
        public string VehicleName { get; set; } = string.Empty;
        public string VehiclePlate { get; set; } = string.Empty;
        public string? EventType { get; set; } 
        public string? Description { get; set; }
        public string? PhotosUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        public string StaffName { get; set; } = string.Empty;
    }
}
