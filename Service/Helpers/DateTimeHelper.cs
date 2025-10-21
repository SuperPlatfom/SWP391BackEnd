
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
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _vnTimeZone);
        }
    }
}
