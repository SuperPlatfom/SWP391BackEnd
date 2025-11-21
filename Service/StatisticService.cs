using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Enums;
using Repository;
using Repository.Interfaces;
using Service.Helpers;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class StatisticService : IStatisticService
    {
        private readonly IAccountRepository _accountRepo;
        private readonly ICoOwnershipGroupRepository _groupRepo;
        private readonly IVehicleRepository _vehicleRepo;
        private readonly IEContractRepository _eContractRepo;
        private readonly IServiceRequestRepository _repo;
        public StatisticService(IAccountRepository accountRepository, ICoOwnershipGroupRepository coOwnershipGroupRepository, IVehicleRepository vehicleRepository, IEContractRepository eContractRepo, IServiceRequestRepository repo)
        {
            _accountRepo = accountRepository;
            _groupRepo = coOwnershipGroupRepository;
            _vehicleRepo = vehicleRepository;
            _eContractRepo = eContractRepo;
            _repo = repo;
        }
        public async Task<StatisticResponse> GetUserStatisticsAsync()
        {
            var accounts = await _accountRepo.GetAllAsync();

            var totalUsers = accounts.Count();

            var byRole = accounts
                .GroupBy(a => a.Role.Name)
                .Select(g => new RoleCountDto
                {
                    Role = g.Key,
                    Count = g.Count()
                })
                .ToList();

            var today = accounts.Count(a => a.CreatedAt.Date == DateTime.Today);
            var thisWeek = accounts.Count(a => a.CreatedAt >= DateTime.Today.AddDays(-7));
            var thisMonth = accounts.Count(a => a.CreatedAt.Month == DateTime.Today.Month);
            var last30Days = accounts.Count(a => a.CreatedAt >= DateTime.Today.AddDays(-30));
            var startOfYear = new DateTime(DateTime.Today.Year, 1, 1);

            var byDate = accounts
                .GroupBy(a => a.CreatedAt.Date)
                .Select(g => new RegistrationByDateDto
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    Count = g.Count()
                })
                .ToList();

           

            return new StatisticResponse
            {
                TotalUsers = totalUsers,
                ByRole = byRole,
                RegistrationStats = new RegistrationStatsDto
                {
                    Today = today,
                    ThisWeek = thisWeek,
                    ThisMonth = thisMonth,
                    Last30Days = last30Days,
                    ThisYear = accounts.Count(a => a.CreatedAt >= startOfYear),
                    ByDate = byDate,
                   
                },
             
                NewVsReturning = new NewVsReturningDto
                {
                    NewUsersIn30Days = last30Days,
                    ReturningUsers = totalUsers - last30Days
                }
            };
        }

        public async Task<RevenueStatisticResponse2> GetRevenueStatisticsAsync()
        {
            var service = await _repo.GetAllAsync();

            var completed = service.Where(c => c.Status == "COMPLETED");

            DateTime today = DateTime.Today;
            DateTime firstDayMonth = new DateTime(today.Year, today.Month, 1);
            DateTime firstDayYear = new DateTime(today.Year, 1, 1);

            var response = new RevenueStatisticResponse2
            {
                Today = new RevenueSection
                {
                    TotalRevenue = completed.Where(c => c.UpdatedAt.Date == today).Sum(c => c.CostEstimate),
                    CompletedOrders = completed.Count(c => c.UpdatedAt.Date == today),
                },
                ThisMonth = new RevenueSection
                {
                    TotalRevenue = completed.Where(c => c.UpdatedAt >= firstDayMonth).Sum(c => c.CostEstimate),
                    CompletedOrders = completed.Count(c => c.UpdatedAt >= firstDayMonth),
                },
                ThisYear = new RevenueSection
                {
                    TotalRevenue = completed.Where(c => c.UpdatedAt >= firstDayYear).Sum(c => c.CostEstimate),
                    CompletedOrders = completed.Count(c => c.UpdatedAt >= firstDayYear),
                },
                ByDate = completed
                    .GroupBy(c => c.UpdatedAt.Date)
                    .Select(g => new RevenueByDate
                    {
                        Date = g.Key.ToString("yyyy-MM-dd"),
                        Revenue = g.Sum(x => x.CostEstimate)
                    }).ToList(),

                ByMonth = completed
                    .GroupBy(c => new { c.UpdatedAt.Year, c.UpdatedAt.Month })
                    .Select(g => new RevenueByMonth
                    {
                        Month = $"{g.Key.Year}-{g.Key.Month:00}",
                        Revenue = g.Sum(x => x.CostEstimate)
                    }).ToList()
            };

            return response;
        }


        public async Task<(bool IsSuccess, string message, RevenueStatisticResponse)> GetRevenueStatisticAsync(DateTime startDate, DateTime endDate)
        {
            var startUtc = DateTimeHelper.ToUtcFromVietnamTime(startDate);
            var endUtc = DateTimeHelper.ToUtcFromVietnamTime(endDate);
            if (startDate >= endDate)
                return (false, "Ngày bắt đầu phải trước ngày kết thúc", null);
            var orders = await _repo.GetCompletedOrdersInRangeAsync(startUtc, endUtc);

            var response = new RevenueStatisticResponse
            {
                TotalRevenue = orders.Sum(x => x.CostEstimate),
                CompletedOrders = orders.Count,
                VehiclesServiced = orders.Select(x => x.VehicleId).Distinct().Count(),
                TechnicianRevenue = orders
                    .GroupBy(x => new { x.TechnicianId, x.Technician.FullName })
                    .Select(g => new TechnicianRevenueModel
                    {
                        TechnicianName = g.Key.FullName,
                        Revenue = g.Sum(o => o.CostEstimate)
                    })
                    .OrderByDescending(x => x.Revenue)
                    .ToList()
            };

            return (true, "Thống kê doanh thu thành công", response);
        }

        public async Task<GroupAndVehicleStatisticResponse> GetGroupVehicleStatisticsAsync()
        {
            var groups = await _groupRepo.GetAllGroupsAsync();
            var vehicles = await _vehicleRepo.GetAllAsync();

            var totalGroups = groups.Count;
            var groupsWithContract = groups.Count(g => g.Contracts.Any());
            var groupsWithoutContract = totalGroups - groupsWithContract;

            var totalVehicles = vehicles.Count();
            var vehiclesWithGroup = vehicles.Count(v => v.GroupId != null);
            var vehiclesWithoutGroup = totalVehicles - vehiclesWithGroup;

            return new GroupAndVehicleStatisticResponse
            {
                TotalGroups = totalGroups,
                GroupsWithContract = groupsWithContract,
                GroupsWithoutContract = groupsWithoutContract,
                TotalVehicles = totalVehicles,
                VehiclesWithGroup = vehiclesWithGroup,
                VehiclesWithoutGroup = vehiclesWithoutGroup
            };
        }

        public async Task<ContractStatisticResponse> GetContractStatisticsAsync()
        {
            var contracts = await _eContractRepo.GetAllAsync();

            var totalContracts = contracts.Count();

            
            var byStatus = contracts
                .GroupBy(x => x.Status)
                .Select(g => new
                {
                    status = g.Key.ToString(),
                    count = g.Count()
                })
                .ToList();

   
            var today = DateTime.UtcNow.Date;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var last30Days = today.AddDays(-30);
            var startOfYear = new DateTime(today.Year, 1, 1);

            var registrationStats = new
            {
                today = contracts.Count(x => x.CreatedAt.Date == today),
                thisWeek = contracts.Count(x => x.CreatedAt.Date >= startOfWeek),
                thisMonth = contracts.Count(x => x.CreatedAt.Date >= startOfMonth),
                last30Days = contracts.Count(x => x.CreatedAt.Date >= last30Days),
                thisYear = contracts.Count(x => x.CreatedAt.Date >= startOfYear),

                byDate = contracts
                    .GroupBy(x => x.CreatedAt.Date)
                    .Select(g => new
                    {
                        date = g.Key.ToString("yyyy-MM-dd"),
                        count = g.Count()
                    })
                    .OrderBy(x => x.date)
                    .ToList()
            };

            return new ContractStatisticResponse
            {
                TotalContracts = totalContracts,
                ByStatus = byStatus,
                RegistrationStats = registrationStats
            };
        }
    }
    }

