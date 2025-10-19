

namespace BusinessObject.DTOs.ResponseModels
{
    public class VehicleResponseModel
    {
        public Guid Id { get; set; }
        public string? PlateNumber { get; set; }
        public string? Make { get; set; }
        public string? Model { get; set; }
        public int ModelYear { get; set; }
        public string? Color { get; set; }
        public string Status { get; set; } = null!;
        public decimal BatteryCapacityKwh { get; set; }
        public string? TelematicsDeviceId { get; set; }
        public int RangeKm { get; set; }
        
        public Guid? GroupId { get; set; }
    }
    public class VehicleOfUserResponseModel
    {
        public Guid Id { get; set; }
        public string? PlateNumber { get; set; }
        public string? Make { get; set; }
        public string? Model { get; set; }
        public int ModelYear { get; set; }
        public string? Color { get; set; }
        public string Status { get; set; } = null!;
        public decimal BatteryCapacityKwh { get; set; }
        public string? TelematicsDeviceId { get; set; }
        public int RangeKm { get; set; }
        public bool HasGroup { get; set; }
    }
}
