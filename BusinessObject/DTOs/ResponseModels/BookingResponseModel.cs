

namespace BusinessObject.DTOs.ResponseModels
{
    public class BookingResponseModel
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public Guid VehicleId { get; set; }
        public Guid UserId { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Purpose { get; set; }
        public string Status { get; set; } = "Booked";

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Nếu bạn muốn hiển thị thêm thông tin tóm tắt
        public string? VehicleName { get; set; }
        public string? GroupName { get; set; }
        public string? UserName { get; set; }
    }
}
