using Repository;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Helpers
{
    public class QuotaResetHelper
    {
        private readonly IUsageQuotaRepository _quotaRepo;

        public QuotaResetHelper( IUsageQuotaRepository quotaRepo)
        {
            _quotaRepo = quotaRepo;
        }

        /// <summary>
        /// Kiểm tra nếu là 0h thứ Hai (giờ Việt Nam) thì reset quota
        /// </summary>
        public async Task CheckAndResetQuotaAsync()
        {
            var nowVN = DateTimeHelper.ToVietnamTime(DateTime.UtcNow);

            if (nowVN.DayOfWeek == DayOfWeek.Monday && nowVN.Hour == 0)
            {
                await _quotaRepo.ResetAllQuotaHoursUsedAsync();
            }
        }
    }
}
