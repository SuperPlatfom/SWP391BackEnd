using BusinessObject.DTOs.ResponseModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IScanningCardService
    {
        Task<string> ScanVietnameseIDCardAsync(IFormFile file);
        Task<ParsedIdCardResult?> ParseVietnameseIdCardAsync(IFormFile file);
    }
}
