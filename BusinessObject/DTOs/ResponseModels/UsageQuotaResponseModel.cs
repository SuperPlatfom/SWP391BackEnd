

namespace BusinessObject.DTOs.ResponseModels
{
    public class UsageQuotaResponseModel
    {
        public Guid VehicleId { get; set; }
        public Guid GroupId { get; set; }
        public DateTime WeekStartDate { get; set; }
        public decimal HoursLimit { get; set; }
        public decimal HoursUsed { get; set; }
        public decimal RemainingHours { get; set; }

    }
}
