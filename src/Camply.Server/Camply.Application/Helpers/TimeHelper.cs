namespace Camply.Application.Helpers
{
    public class TimeHelper
    {
        public static string GetTimeExisted(DateTime createdDate)
        {
            var now = DateTime.UtcNow;
            var span = now - createdDate;

            if (span.TotalDays < 1)
                return $"{span.Hours}h";
            if (span.TotalDays < 30)
                return $"{(int)span.TotalDays}d";
            if (span.TotalDays < 365)
                return $"{(int)(span.TotalDays / 30)}mo";
    
            return $"{(int)(span.TotalDays / 365)}y";
        }

    }
}