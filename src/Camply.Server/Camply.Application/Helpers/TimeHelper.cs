namespace Camply.Application.Helpers
{
    public class TimeHelper
    {
        public static string GetTimeExisted(DateTime createdDate)
        {
            var now = DateTime.UtcNow;
            var span = now - createdDate;

            if (span.TotalSeconds < 60)
                return $"{(int)span.TotalSeconds}s";
            if (span.TotalMinutes < 60)
                return $"{(int)span.TotalMinutes}m";
            if (span.TotalHours < 24)
                return $"{(int)span.TotalHours}h";
            if (span.TotalDays < 30)
                return $"{(int)span.TotalDays}d";
            if (span.TotalDays < 365)
                return $"{(int)(span.TotalDays / 30)}mo";

            return $"{(int)(span.TotalDays / 365)}y"; 
        }

    }
}