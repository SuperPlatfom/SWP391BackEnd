using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.ResponseModels
{
    public class UsageQuotaResponseModel
    {
        public Guid VehicleId { get; set; }
        public Guid GroupId { get; set; }
        public DateTime WeekStartDate { get; set; }
        public decimal HoursLimit { get; set; }
        public decimal HoursUsed { get; set; }
        public decimal HoursDebt { get; set; }
        public decimal HoursAdvance { get; set; }

        public decimal RemainingHours { get; set; }

        public decimal RemainingHoursNextWeek { get; set; }
    }
}
