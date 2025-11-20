using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using BusinessObject.RequestModels;
using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.Interfaces;
using Service.Interfaces;
using System.Security.Claims;


namespace Service
{
    public class VehicleRequestService : IVehicleRequestService
    {
        private readonly IVehicleRequestRepository _vehicleRequestRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IFirebaseStorageService _storageService;
        private readonly INotificationService _notificationService;
        public VehicleRequestService(IVehicleRequestRepository vehicleRequestRepository, IFirebaseStorageService storageService, IVehicleRepository vehicleRepository, INotificationService notificationService)
        {
            _vehicleRequestRepository = vehicleRequestRepository;
            _storageService = storageService;
            _vehicleRepository = vehicleRepository;
            _notificationService = notificationService;
        }

        public async Task<IEnumerable<VehicleRequestResponseModel>> GetAllRequestsAsync()
        {
            var requests = await _vehicleRequestRepository.GetAllRequestsAsync();
            return requests.Select(MapToResponseModel);
        }

        public async Task<IEnumerable<VehicleRequestResponseModel>> GetMyRequestsAsync(ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Không thể xác định người dùng.");

            var requests = await _vehicleRequestRepository.GetRequestsByUserAsync(Guid.Parse(userId));
            return requests.Select(MapToResponseModel);
        }

        public async Task<VehicleRequestResponseModel> GetRequestDetailAsync(Guid id)
        {
            var request = await _vehicleRequestRepository.GetByIdWithDetailAsync(id);
            if (request == null)
                throw new KeyNotFoundException("Không tìm thấy yêu cầu này.");
            return MapToResponseModel(request);
        }

        public async Task<(bool IsSuccess, string Message)> CreateVehicleRequestAsync(VehicleRequestModel request, ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return (false, "Không thể xác định người dùng.");

            if (!Guid.TryParse(userId, out var creatorId))
               return (false, "Token không hợp lệ.");

            if (request.vehicleImage == null || request.registrationPaperUrl == null)
              return (false, "Vui lòng upload đầy đủ ảnh xe và giấy tờ xe.");

            bool exists = await _vehicleRepository.ExistsAsync(v => v.PlateNumber == request.plateNumber);
            if (exists)
                return (false, "Biển số xe đã tồn tại trong hệ thống.");

            bool exists2 = await _vehicleRequestRepository.ExistsAsync(vs => vs.PlateNumber == request.plateNumber && vs.Status == "PENDING");
            if (exists2)
                return (false, "Biển số xe này đang được duyệt.");


            List<string> vehicleImageUrls = new();
            if (request.vehicleImage.Count > 4)
                return (false, "Chỉ được tải lên tối đa 4 hình ảnh xe.");
            foreach (var image in request.vehicleImage)
            {
                var url = await _storageService.UploadFileAsync(image, "vehicle");
                vehicleImageUrls.Add(url);
            }
                var registrationPaperUrl = await _storageService.UploadFileAsync(request.registrationPaperUrl, "registration");


            var vehicleRequest = new VehicleRequest
            {
                Id = Guid.NewGuid(),
                PlateNumber = request.plateNumber,
                Make = request.make,
                Model = request.model,
                ModelYear = request.modelYear,
                Color = request.color,
                BatteryCapacityKwh = request.batteryCapacityKwh,
                RangeKm = request.rangeKm,
                VehicleImageUrl = vehicleImageUrls.Any()
                                     ? System.Text.Json.JsonSerializer.Serialize(vehicleImageUrls)
    : null,
                RegistrationPaperUrl = registrationPaperUrl,
                Type = "CREATE",
                Status = "PENDING",
                CreatedBy = Guid.Parse(userId),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };


            await _vehicleRequestRepository.AddAsync(vehicleRequest);
           return (true, "Đã gửi yêu cầu đăng kí xe, chờ duyệt.");
        }
        public async Task<(bool IsSuccess, string Message)> UpdateVehicleRequestAsync(VehicleUpdateModel model, ClaimsPrincipal user)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(model.VehicleId);
            if (vehicle == null)
            {
                return (false, "Không tìm thấy xe cần cập nhật.");
            }
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return (false, "Không thể xác định người dùng.");

            if (!Guid.TryParse(userId, out var creatorId))
                return (false, "Token không hợp lệ.");

            if (model.vehicleImage == null || model.registrationPaperUrl == null)
                return (false, "Vui lòng upload đầy đủ ảnh xe và giấy tờ xe.");

            if (model.plateNumber != vehicle.PlateNumber)
            {
                bool exists = await _vehicleRepository.ExistsAsync(v => v.PlateNumber == model.plateNumber && v.Id != vehicle.Id);
                if (exists)
                    return (false, "Biển số xe đã tồn tại trong hệ thống.");
            }

            bool exists2 = await _vehicleRequestRepository.ExistsAsync(vs => vs.PlateNumber == model.plateNumber && vs.Status == "PENDING");
            if (exists2)
                return (false, "Biển số xe này đang được duyệt.");


            List<string> vehicleImageUrls = new();
            if (model.vehicleImage.Count > 4)
                return (false, "Chỉ được tải lên tối đa 4 hình ảnh xe.");
            foreach (var image in model.vehicleImage)
            {
                var url = await _storageService.UploadFileAsync(image, "vehicle");
                vehicleImageUrls.Add(url);
            }
            var registrationPaperUrl = await _storageService.UploadFileAsync(model.registrationPaperUrl, "registration");

           
            var request = new VehicleRequest
            {
                Id = Guid.NewGuid(),
                VehicleId = vehicle.Id,
                PlateNumber = model.plateNumber,
                Make = model.Make,
                Model = model.Model,
                ModelYear = model.modelYear,
                Color = model.color,
                BatteryCapacityKwh = model.batteryCapacityKwh,
                RangeKm = model.rangeKm,
                VehicleImageUrl = vehicleImageUrls.Any()
                                     ? System.Text.Json.JsonSerializer.Serialize(vehicleImageUrls)
    : null,
                RegistrationPaperUrl = registrationPaperUrl,
                Type = "UPDATE",
                Status = "PENDING",
                CreatedBy = Guid.Parse(userId),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };


            await _vehicleRequestRepository.AddAsync(request);

            return (true, "Đã gửi yêu cầu cập nhật xe, chờ duyệt.");
        }

        private VehicleRequestResponseModel MapToResponseModel(VehicleRequest req)
        {
            return new VehicleRequestResponseModel
            {
                Id = req.Id,
                VehicleId = req.VehicleId,
                PlateNumber = req.PlateNumber,
                Make = req.Make,
                Model = req.Model,
                ModelYear = req.ModelYear,
                Color = req.Color,
                BatteryCapacityKwh = req.BatteryCapacityKwh,
                RangeKm = req.RangeKm,
                VehicleImageUrl = req.VehicleImageUrl,
                RegistrationPaperUrl = req.RegistrationPaperUrl,
                Type = req.Type,
                Status = req.Status,
                RejectionReason = req.RejectionReason,
                CreatedBy = req.CreatedBy,
                CreatedByName = req.Requester?.FullName,
                CreatedAt = req.CreatedAt,
                UpdatedAt = req.UpdatedAt
            };
        }

        public async Task<(bool IsSuccess, string Message)> ApproveUpdateRequestAsync(Guid requestId, ClaimsPrincipal user)
        {
            var request = await _vehicleRequestRepository.GetByIdWithDetailAsync(requestId);
            if (request == null)
                return (false, "Không tìm thấy yêu cầu cập nhật.");

            if (request.Type != "UPDATE")
                return (false, "Yêu cầu này không phải là loại UPDATE.");

            if (request.Status != "PENDING")
                return (false, "Chỉ có thể duyệt yêu cầu đang ở trạng thái PENDING.");

            var vehicle = await _vehicleRepository.GetByIdAsync(request.VehicleId ?? Guid.Empty);
            if (vehicle == null)
                return (false, "Không tìm thấy xe cần cập nhật.");


            bool exists = await _vehicleRepository.ExistsAsync(v => v.PlateNumber == request.PlateNumber);
            if (exists)
                throw new InvalidOperationException("Biển số xe đã tồn tại trong hệ thống.");


            vehicle.PlateNumber = request.PlateNumber;
            vehicle.Make = request.Make;
            vehicle.Model = request.Model;
            vehicle.ModelYear = request.ModelYear;
            vehicle.Color = request.Color;
            vehicle.BatteryCapacityKwh = request.BatteryCapacityKwh;
            vehicle.RangeKm = request.RangeKm;
            vehicle.VehicleImageUrl = request.VehicleImageUrl;
            vehicle.RegistrationPaperUrl = request.RegistrationPaperUrl;
            vehicle.UpdatedAt = DateTime.UtcNow;

            await _vehicleRepository.UpdateAsync(vehicle);

            request.Status = "APPROVED";
            request.UpdatedAt = DateTime.UtcNow;
            await _vehicleRequestRepository.UpdateAsync(request);

            await _notificationService.CreateAsync(vehicle.CreatedBy, "Cập nhật thông tin thành công", "Yêu cầu cập nhật thông tin xe của bạn đã được phê duyệt", "UPDATE VEHICLE REQUEST", requestId);

            return (true, "Phê duyệt yêu cầu cập nhật xe thành công.");
        }

        public async Task<(bool IsSuccess, string Message)> ApproveRequestAsync(Guid requestId, ClaimsPrincipal user)
        {
            var request = await _vehicleRequestRepository.GetByIdWithDetailAsync(requestId);
            if (request == null)
                return (false, "Không tìm thấy yêu cầu.");

            if (request.Status != "PENDING")
                return (false, "Yêu cầu này đã được xử lý trước đó.");

            bool exists = await _vehicleRepository.ExistsAsync(v => v.PlateNumber == request.PlateNumber);
            if (exists)
                throw new InvalidOperationException("Biển số xe đã tồn tại trong hệ thống.");

            // Tạo Vehicle mới dựa trên VehicleRequest
            var vehicle = new Vehicle
            {
                Id = Guid.NewGuid(),
                PlateNumber = request.PlateNumber,
                Make = request.Make,
                Model = request.Model,
                ModelYear = request.ModelYear,
                Color = request.Color,
                BatteryCapacityKwh = request.BatteryCapacityKwh,
                RangeKm = request.RangeKm,
                Status = "INACTIVE",
                VehicleImageUrl = request.VehicleImageUrl,
                RegistrationPaperUrl = request.RegistrationPaperUrl,
                WeeklyQuotaHours = 112,
                GroupId = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = request.CreatedBy,

            };

            await _vehicleRepository.AddAsync(vehicle);

            // Cập nhật trạng thái request
            request.Status = "APPROVED";
            request.VehicleId = vehicle.Id;
            request.UpdatedAt = DateTime.UtcNow;

            await _vehicleRequestRepository.UpdateAsync(request);

            await _notificationService.CreateAsync(vehicle.CreatedBy, "đăng kí xe thành công", "Yêu cầu đăng kí xe của bạn đã được phê duyệt", "CREATE VEHICLE REQUEST", requestId);
            return (true, "Phê duyệt yêu cầu thành công.");
        }

        public async Task<(bool IsSuccess, string Message)> RejectRequestAsync(Guid requestId, string reason, ClaimsPrincipal user)
        {
            var request = await _vehicleRequestRepository.GetByIdWithDetailAsync(requestId);
            if (request == null)
                return (false, "Không tìm thấy yêu cầu.");

            if (request.Status != "PENDING")
                return (false, "Yêu cầu này đã được xử lý trước đó.");

            request.Status = "REJECTED";
            request.RejectionReason = reason;
            request.UpdatedAt = DateTime.UtcNow;

            await _vehicleRequestRepository.UpdateAsync(request);

            if (request.Type == "CREATE")
            {
                await _notificationService.CreateAsync(request.CreatedBy, "Yêu cầu bị từ chối", "Yêu cầu cập nhật thông tin xe của bạn đã bị từ chối, vui lòng xem lý do để biết thông tin chi tiết", "CREATE REQUEST", requestId);
                return (true, "Đã từ chối yêu cầu đăng kí xe.");
            } else
            {
                await _notificationService.CreateAsync(request.CreatedBy, "Yêu cầu bị từ chối", "Yêu cầu cập nhật thông tin xe của bạn đã bị từ chối, vui lòng xem lý do để biết thông tin chi tiết", "UPDATE REQUEST", requestId);
                return (true, "Đã từ chối yêu cầu cập nhật xe.");
            }
                
        }

        public async Task<(bool IsSuccess, string Message)> DeleteVehicleRequestAsync(Guid requestId, ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return (false, "Không thể xác định người dùng.");

            if (!Guid.TryParse(userId, out var ownerId))
                return (false, "Token không hợp lệ.");

            var request = await _vehicleRequestRepository.GetByIdWithDetailAsync(requestId);
            if (request == null)
                return (false, "Không tìm thấy yêu cầu cần xoá.");

            // Chỉ cho phép xoá khi request thuộc người hiện tại và chưa được duyệt
            if (request.CreatedBy != ownerId)
                return (false, "Bạn không có quyền xoá yêu cầu này.");

            if (request.Status == "APPROVED" || request.Status == "REJECTED")
                return (false, "Không thể xoá yêu cầu đã được duyệt hoặc từ chối.");

            await _vehicleRequestRepository.DeleteAsync(requestId);

            return (true, "Đã xoá yêu cầu thành công.");
        }
    }
}
