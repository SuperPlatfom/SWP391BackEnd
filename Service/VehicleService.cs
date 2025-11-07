using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using Repository.Interfaces;
using Service.Interfaces;
using System.Security.Claims;

namespace Service
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IVehicleRequestRepository _vehicleRequestRepository;
    
        public VehicleService(IVehicleRepository vehicleRepository, IVehicleRequestRepository vehicleRequestRepository)
        {
            _vehicleRepository = vehicleRepository;
            _vehicleRequestRepository = vehicleRequestRepository;
        }

        public async Task<IEnumerable<VehicleResponseModel>> GetAllVehiclesAsync()
        {
            var vehicles = await _vehicleRepository.GetAllAsync();

           
            var result = vehicles.Select(v => new VehicleResponseModel
            {
                Id = v.Id,
                PlateNumber = v.PlateNumber,
                Make = v.Make,
                Model = v.Model,
                ModelYear = v.ModelYear,
                Color = v.Color,
                Status = v.Status,
                BatteryCapacityKwh = v.BatteryCapacityKwh,
                RangeKm = v.RangeKm,
                GroupId = v.GroupId,
                VehicleImageUrl = v.VehicleImageUrl,
                RegistrationPaperUrl = v.RegistrationPaperUrl
            });

            return result;
        }

        public async Task<VehicleResponseModel?> GetVehicleByIdAsync(Guid id)
        {
            var v = await _vehicleRepository.GetByIdAsync(id);
            if (v == null)
                return null;

            return new VehicleResponseModel
            {
                Id = v.Id,
                PlateNumber = v.PlateNumber,
                Make = v.Make,
                Model = v.Model,
                ModelYear = v.ModelYear,
                Color = v.Color,
                Status = v.Status,
                BatteryCapacityKwh = v.BatteryCapacityKwh,
                RangeKm = v.RangeKm,
                GroupId = v.GroupId,
                VehicleImageUrl = v.VehicleImageUrl,
                RegistrationPaperUrl = v.RegistrationPaperUrl,
            };
        }

       


        private VehicleResponseModel MapToResponseModel(Vehicle vehicle)
        {
            return new VehicleResponseModel
            {
                Id = vehicle.Id,
                PlateNumber = vehicle.PlateNumber,
                Make = vehicle.Make,
                Model = vehicle.Model,
                ModelYear = vehicle.ModelYear,
                Color = vehicle.Color,
                Status = vehicle.Status,
                BatteryCapacityKwh = vehicle.BatteryCapacityKwh,
                RangeKm = vehicle.RangeKm,
                VehicleImageUrl = vehicle.VehicleImageUrl,
                RegistrationPaperUrl = vehicle.RegistrationPaperUrl,
            };
        }

       

        public async Task<(bool isSuccess, string Message)> DeleteVehicleAsync(Guid id)
        {
            var existing = await _vehicleRepository.GetByIdAsync(id);
            if (existing == null)
                return (false, "Ko tìm thấy xe");
            if (existing.GroupId != null)
            {
               
                return (false, "Không thể xoá xe này, xe này đã có nhóm");

            }
            else
            {
                await _vehicleRepository.DeleteAsync(id);
                return (true, "Xoá xe thành công");
            }
        }


        public async Task<List<VehicleOfUserResponseModel>> GetVehiclesByCreatorAsync(ClaimsPrincipal user)
        {
            var userIdStr = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr))
                throw new UnauthorizedAccessException("Không xác định được người dùng.");

            var userId = Guid.Parse(userIdStr);

            var vehicles = await _vehicleRepository.GetVehiclesByCreatorAsync(userId);
            return vehicles.Select(v => new VehicleOfUserResponseModel
            {
                Id = v.Id,
                Make = v.Make,
                Model = v.Model,
                ModelYear = v.ModelYear,
                Color = v.Color,
                PlateNumber = v.PlateNumber,
                Status = v.Status,
                BatteryCapacityKwh = v.BatteryCapacityKwh,
                RangeKm = v.RangeKm,
               HasGroup = v.GroupId !=null,
               VehicleImageUrl = v.VehicleImageUrl,
               RegistrationPaperUrl = v.RegistrationPaperUrl,
            }).ToList();
        }


    }
}