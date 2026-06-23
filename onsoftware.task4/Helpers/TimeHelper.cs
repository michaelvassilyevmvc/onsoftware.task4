namespace onsoftware.task4.Helpers;

public static class TimeHelper
{
    public static string ToRelativeTime(DateTime dateTime)
    {
        var diff = DateTime.UtcNow - dateTime;

        if (diff.TotalSeconds < 60)
            return "less than a minute ago";  

        if (diff.TotalMinutes < 60)
            return $"{(int)diff.TotalMinutes} min ago";

        if (diff.TotalHours < 24)
            return $"{(int)diff.TotalHours} hours ago";

        if (diff.TotalDays < 30)
            return $"{(int)diff.TotalDays} days back";

        if (diff.TotalDays < 365)
            return $"{(int)(diff.TotalDays / 30)} months back";

        return $"{(int)(diff.TotalDays / 365)} years ago";
    }
}