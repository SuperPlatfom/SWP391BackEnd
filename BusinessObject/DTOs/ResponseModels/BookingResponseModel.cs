

namespace BusinessObject.DTOs.ResponseModels
{
    public class BookingResponseModel
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public Guid VehicleId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; } = "BOOKED";

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

     
    }
}
