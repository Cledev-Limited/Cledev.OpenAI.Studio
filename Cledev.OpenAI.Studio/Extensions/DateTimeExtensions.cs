namespace Cledev.OpenAI.Studio.Extensions;

public static class DateTimeExtensions
{
    public static DateTime ToDateTime(this int unixTimeStamp)
    {
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dateTime;
    }

    public static DateTime ToDateTime(this int? unixTimeStamp, int fallBack)
    {
        unixTimeStamp ??= fallBack;
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp.Value).ToLocalTime();
        return dateTime;
    }
}
