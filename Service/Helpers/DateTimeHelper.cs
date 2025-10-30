
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


        public static DateTime NowVietnamTime()
        {

            var utcNow = DateTime.UtcNow;
            var vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            return DateTime.SpecifyKind(vietnamTime, DateTimeKind.Local); 
        }

        public static DateTime ToUtcFromVietnamTime(DateTime vietnamTime)
        {
            return TimeZoneInfo.ConvertTimeToUtc(vietnamTime, _vnTimeZone);
        }

        public static DateTime GetWeekStartDate(DateTime vietnamNow)
        {
            int diff = (7 + (vietnamNow.DayOfWeek - DayOfWeek.Monday)) % 7;
            return vietnamNow.Date.AddDays(-diff);
        }
    }
}
