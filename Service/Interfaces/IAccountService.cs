using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IAccountService
    {

        Task AddAsync(AddAccountRequestModel userDto, string roleName);
        Task<IEnumerable<AccountResponseModel>> GetAllAsync(string? role = null);
    }
}
