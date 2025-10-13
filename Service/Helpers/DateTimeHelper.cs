using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace Service.Helpers
{
    public static class DateTimeHelper
    {
        private static readonly TimeZoneInfo _vnTimeZone = TZConvert.GetTimeZoneInfo("SE Asia Standard Time");

        public static DateTime ToVietnamTime(DateTime utcTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, _vnTimeZone);
        }
    }
}
