using BusinessObject.DTOs.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IStatisticService
    {
        Task<StatisticResponse> GetUserStatisticsAsync();
        Task<GroupAndVehicleStatisticResponse> GetGroupVehicleStatisticsAsync();
        Task<ContractStatisticResponse> GetContractStatisticsAsync();
        Task<(bool IsSuccess, string message, RevenueStatisticResponse)> GetRevenueStatisticAsync(DateTime startDate, DateTime endDate);
        Task<RevenueStatisticResponse2> GetRevenueStatisticsAsync();
    }
}
