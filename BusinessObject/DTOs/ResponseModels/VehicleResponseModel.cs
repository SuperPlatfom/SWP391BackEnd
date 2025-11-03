

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
        public int RangeKm { get; set; }
        
        public Guid? GroupId { get; set; }
        public string? VehicleImageUrl { get; set; }
        public string? RegistrationPaperUrl { get; set; }
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

    public class VehicleRequestResponseModel
    {
        public Guid Id { get; set; }

      
        public Guid? VehicleId { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int ModelYear { get; set; }
        public string Color { get; set; } = string.Empty;
        public decimal BatteryCapacityKwh { get; set; }
        public int RangeKm { get; set; }

     
        public string VehicleImageUrl { get; set; } = string.Empty;
        public string RegistrationPaperUrl { get; set; } = string.Empty;

     
        public string Type { get; set; } = "CREATE"; 
        public string Status { get; set; } = "PENDING"; 
        public string? RejectionReason { get; set; }

       
        public Guid CreatedBy { get; set; }
        public string? CreatedByName { get; set; } 
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
