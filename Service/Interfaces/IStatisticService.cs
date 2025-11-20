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
    }
}
