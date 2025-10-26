
using TimeZoneConverter;

namespace Service.Helpers
{
    public static class DateTimeHelper
    {
        public static TimeZoneInfo GetVietnamTimeZone()
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
            }
            catch (TimeZoneNotFoundException)
            {
                return TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            }
        }
        private static readonly TimeZoneInfo _vnTimeZone = TZConvert.GetTimeZoneInfo("SE Asia Standard Time");

      /*  public static DateTime ToVietnamTime(DateTime utcTime)
        {
            var tz = GetVietnamTimeZone();
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, tz);
        }
      */
        public static DateTime NowVietnamTime()
        {

            var utcNow = DateTime.UtcNow;
            var vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            return DateTime.SpecifyKind(vietnamTime, DateTimeKind.Utc); 
        }

        public static DateTime ToVietnamTime(DateTime utcTime)
        {
            if (utcTime.Kind == DateTimeKind.Unspecified)
                utcTime = DateTime.SpecifyKind(utcTime, DateTimeKind.Utc);
            var tz = GetVietnamTimeZone();
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, tz);
        }

        public static DateTime ToUtcFromVietnamTime(DateTime vietnamTime)
        {
            if (vietnamTime.Kind == DateTimeKind.Unspecified)
                vietnamTime = DateTime.SpecifyKind(vietnamTime, DateTimeKind.Unspecified);
            var tz = GetVietnamTimeZone();
            return TimeZoneInfo.ConvertTimeToUtc(vietnamTime, tz);
        }
    }
}
