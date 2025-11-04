

namespace BusinessObject.DTOs.ResponseModels
{
    public class TripDamageReportResponse
    {
        public Guid Id { get; set; }
        public Guid VehicleId { get; set; }
        public string VehiclePlate { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? PhotosUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public string StaffName { get; set; } = string.Empty;
    }
}
