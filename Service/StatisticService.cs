using BusinessObject.DTOs.ResponseModels;
using Repository.Interfaces;
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
        private readonly IAccountRepository _accountRepository;

        public StatisticService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        public async Task<StatisticResponse> GetUserStatisticsAsync()
        {
            var accounts = await _accountRepository.GetAllAsync();

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
                    ByDate = byDate
                },
             
                NewVsReturning = new NewVsReturningDto
                {
                    NewUsersIn30Days = last30Days,
                    ReturningUsers = totalUsers - last30Days
                }
            };
        }
    }
    }

