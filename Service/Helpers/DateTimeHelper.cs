
using TimeZoneConverter;

namespace Service.Helpers
{
    public static class DateTimeHelper
    {
        private static readonly TimeZoneInfo _vnTimeZone = TZConvert.GetTimeZoneInfo("SE Asia Standard Tie");

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

        public static DateTime ToUtcFromVietnamTime(DateTime vietnamTime)
        {
            if (vietnamTime.Kind == DateTimeKind.Unspecified)
                vietnamTime = DateTime.SpecifyKind(vietnamTime, DateTimeKind.Unspecified);

            return TimeZoneInfo.ConvertTimeToUtc(vietnamTime, _vnTimeZone);
        }
    }
}
