using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.ResponseModels
{
    public class StatisticResponse
    {
        public int TotalUsers { get; set; }
        public List<RoleCountDto> ByRole { get; set; }
        public RegistrationStatsDto RegistrationStats { get; set; }
        public ActiveInactiveDto ActiveInactive { get; set; }
        public NewVsReturningDto NewVsReturning { get; set; }
    }

    public class RoleCountDto
    {
        public string Role { get; set; }
        public int Count { get; set; }
    }

    public class RegistrationStatsDto
    {
        public int Today { get; set; }
        public int ThisWeek { get; set; }
        public int ThisMonth { get; set; }
        public int Last30Days { get; set; }
        public List<RegistrationByDateDto> ByDate { get; set; }
        public int ThisYear { get; set; }
    }

    public class RegistrationByDateDto
    {
        public string Date { get; set; }
        public int Count { get; set; }
    }

    public class ActiveInactiveDto
    {
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
    }

    public class NewVsReturningDto
    {
        public int NewUsersIn30Days { get; set; }
        public int ReturningUsers { get; set; }
    }


    public class RevenueStatisticResponse
    {

        public decimal? TotalRevenue { get; set; }
        public int? CompletedOrders { get; set; }
        public int? VehiclesServiced { get; set; }
        public List<TechnicianRevenueModel?> TechnicianRevenue { get; set; }
    }

    public class TechnicianRevenueModel
    {
        public string TechnicianName { get; set; }
        public decimal? Revenue { get; set; }
    }

    public class GroupAndVehicleStatisticResponse
    {
        public int TotalGroups { get; set; }
        public int GroupsWithContract { get; set; }
        public int GroupsWithoutContract { get; set; }

        public int TotalVehicles { get; set; }
        public int VehiclesWithGroup { get; set; }
        public int VehiclesWithoutGroup { get; set; }
    }

    public class ContractStatisticResponse
    {
        public int TotalContracts { get; set; }

        public object ByStatus { get; set; }

        public object RegistrationStats { get; set; }
    }

    public class RevenueStatisticResponse2
    {
        public RevenueSection Today { get; set; }
        public RevenueSection ThisMonth { get; set; }
        public RevenueSection ThisYear { get; set; }

        public List<RevenueByDate> ByDate { get; set; }
        public List<RevenueByMonth> ByMonth { get; set; }
    }

    public class RevenueSection
    {
        public decimal? TotalRevenue { get; set; }
        public int CompletedOrders { get; set; }
    }

    public class RevenueByDate
    {
        public string Date { get; set; }
        public decimal? Revenue { get; set; }
    }

    public class RevenueByMonth
    {
        public string Month { get; set; }
        public decimal? Revenue { get; set; }
    }
}
