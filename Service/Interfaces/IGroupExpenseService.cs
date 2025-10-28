using BusinessObject.DTOs.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IGroupExpenseService
    {
        Task<GroupExpenseDto> CreateFromApprovedRequestAsync(Guid serviceRequestId);
        Task<IEnumerable<GroupExpenseDto>> GetByGroupAsync(Guid groupId);
    }

}
