using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using Repository.Interfaces;
using Service.Helpers;
using Service.Interfaces;

namespace Service
{
    public class ServiceCenterService : IServiceCenterService
    {
        private readonly IServiceCenterRepository _repo;

        public ServiceCenterService(IServiceCenterRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<ServiceCenterDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();

            return list.Select(c => new ServiceCenterDto
            {
                Id = c.Id,
                Name = c.Name,
                Address = c.Address,
                Phone = c.Phone,
                IsActive = c.IsActive,
                CreatedAt = DateTimeHelper.ToVietnamTime(c.CreatedAt),
                UpdatedAt = DateTimeHelper.ToVietnamTime(c.UpdatedAt)
            }).ToList();
        }

        public async Task<ServiceCenterDto> GetByIdAsync(Guid id)
        {
            var center = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Không tìm thấy trung tâm dịch vụ");

            return new ServiceCenterDto
            {
                Id = center.Id,
                Name = center.Name,
                Address = center.Address,
                Phone = center.Phone,
                IsActive = center.IsActive,
                CreatedAt = DateTimeHelper.ToVietnamTime(center.CreatedAt),
                UpdatedAt = DateTimeHelper.ToVietnamTime(center.UpdatedAt)
            };
        }

        public async Task<ServiceCenterDto> CreateAsync(CreateServiceCenterRequest req)
        {
            var entity = new ServiceCenter
            {
                Id = Guid.NewGuid(),
                Name = req.Name,
                Address = req.Address,
                Phone = req.Phone,
                IsActive = req.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            return new ServiceCenterDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Address = entity.Address,
                Phone = entity.Phone,
                IsActive = entity.IsActive,
                CreatedAt = DateTimeHelper.ToVietnamTime(entity.CreatedAt),
                UpdatedAt = DateTimeHelper.ToVietnamTime(entity.UpdatedAt)
            };
        }

        public async Task<ServiceCenterDto> UpdateAsync(Guid id, CreateServiceCenterRequest req)
        {
            var entity = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Không tìm thấy trung tâm dịch vụ");

            entity.Name = req.Name;
            entity.Address = req.Address;
            entity.Phone = req.Phone;
            entity.IsActive = req.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(entity);
            await _repo.SaveChangesAsync();

            return new ServiceCenterDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Address = entity.Address,
                Phone = entity.Phone,
                IsActive = entity.IsActive,
                CreatedAt = DateTimeHelper.ToVietnamTime(entity.CreatedAt),
                UpdatedAt = DateTimeHelper.ToVietnamTime(entity.UpdatedAt)
            };
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Không tìm thấy trung tâm dịch vụ");

            await _repo.DeleteAsync(id);
            await _repo.SaveChangesAsync();
        }
    }
}
