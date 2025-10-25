using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IServiceCenterService
    {
        Task<IEnumerable<ServiceCenterDto>> GetAllAsync();
        Task<ServiceCenterDto> GetByIdAsync(Guid id);
        Task<ServiceCenterDto> CreateAsync(CreateServiceCenterRequest req);
        Task<ServiceCenterDto> UpdateAsync(Guid id, CreateServiceCenterRequest req);
        Task DeleteAsync(Guid id);
    }
}
