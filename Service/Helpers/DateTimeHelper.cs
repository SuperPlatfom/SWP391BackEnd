
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
            return DateTime.SpecifyKind(vietnamTime, DateTimeKind.Utc); 
        }
    }
}
