using System.Globalization;
using AeroBlazor.Services;
using Columbae;

namespace AeroBlazor.Extensions;

public static class DisplayExtensions
{
    public static string PrintHistoricalTime(this DateTimeOffset? utcTimestamp, TranslatorService translatorService)
    {
        if (!utcTimestamp.HasValue) return "-";
        return utcTimestamp.Value.PrintHistoricalTime(translatorService);
    }

    public static string PrintHistoricalTime(this DateTimeOffset utcTimestamp, TranslatorService translatorService)
    {
        // TODO : pick time zone from current browser
        return utcTimestamp.DateTime.PrintHistoricalTime(translatorService);
    }

    public static string PrintHistoricalTime(this DateTime utcTimestamp, TranslatorService translatorService)
    {
        var localTime = TimeZoneInfo.ConvertTime(utcTimestamp, TimeZoneInfo.Local);
        var currentDate = DateTime.UtcNow;
        if (currentDate <= utcTimestamp)
        {
            return translatorService["time/now"];
        }

        var timeAgo = currentDate - utcTimestamp;
        // If less than a minute
        if (timeAgo < TimeSpan.FromMinutes(1))
        {
            return string.Format(translatorService["time/secondsago"], timeAgo.Seconds);
        }

        // If less than an hour
        if (timeAgo < TimeSpan.FromHours(1))
        {
            return string.Format(translatorService["time/minutesago"], timeAgo.Minutes);
        }

        // If today
        if (timeAgo < TimeSpan.FromHours(24) && currentDate.Day == utcTimestamp.Day)
        {
            return $"{translatorService["time/today"]} {localTime:HH:mm}";
        }

        // If this week
        if (timeAgo < TimeSpan.FromDays(7))
        {
            return
                $"{CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(localTime.DayOfWeek)} {localTime:HH:mm}";
        }

        // Just time print
        return localTime.ToString("g");
    }
    
    public static string PrintHistoricalDate(this DateTime utcTimestamp, TranslatorService translatorService)
    {
        // Calculate the difference in days between the input date and today
        var localTime = TimeZoneInfo.ConvertTime(utcTimestamp, TimeZoneInfo.Local);
        int daysDifference = (localTime - DateTime.Today).Days;

        // Determine the description based on the difference in days
        return daysDifference switch
        {
            -1 => translatorService["time/now"],
            0 => translatorService["Today"],
            1 => translatorService["Tomorrow"],
            _ when daysDifference < 0 => $"{Math.Abs(daysDifference)} {translatorService["days ago"]}",
            _ => $"{string.Format( translatorService["in {0} days"],  daysDifference)}"
        };
    }

    public static string Print(this DateTimeOffset? utcTimestamp, bool shortFormat = false)
    {
        if (utcTimestamp == null) return "-";
        var format = shortFormat ? "g" : "f";
        return utcTimestamp.Value.ToString(format);
    }

    public static string PrintDate(this DateTimeOffset? utcTimestamp, bool shortFormat = false)
    {
        if (utcTimestamp == null) return "-";
        var format = shortFormat ? "d" : "D";
        return utcTimestamp.Value.ToString(format);
    }
    
    public static string PrintNumber(this double value, string? suffix = null)
    {
        return $"{value:N2}{suffix}";
    }

    public static string PrintBoolean(this bool? value)
    {
        if (value == null) return "-";
        return PrintBoolean(value.Value);
    }

    public static string PrintLocation(this Polypoint? coordinates)
    {
        if (coordinates == null) return "-";
        if (coordinates.Latitude == 0) return "-";
        if (coordinates.Longitude == 0) return "-";
        var latDirection = coordinates.Latitude > 0 ? "N" : "S";
        var lonDirection = coordinates.Longitude > 0 ? "E" : "W";
        return $"{coordinates.Latitude:0.0000} {latDirection} - {coordinates.Longitude:0.0000} {lonDirection}";
    }


    public static string PrintBoolean(this bool value)
    {
        return value ? "true" : "false";
    }


    public static string PrintAvatar(this string? fullText, string emptyValue = "?")
    {
        if (string.IsNullOrEmpty(fullText)) return emptyValue;
        fullText = fullText.Trim();
        var parts = fullText.Split(' ');
        parts = parts.Where(p => !p.StartsWith($"(")).ToArray();
        if (parts.Length == 1) return fullText.First().ToString().ToUpper();
        return $"{parts.First().First()}{parts.Last().First()}".ToUpper();
    }
    
    public static string PrintDistance(this double distanceInMeter, TranslatorService localizer, bool includeDistanceSymbol = false)
    {
        var distanceSymbol = includeDistanceSymbol ? "" : " ↔";
        if (distanceInMeter < 0) return "";
        if(distanceInMeter < 50) return $"{localizer["Nearby"]}{distanceSymbol}";
        if(distanceInMeter < 1000) return $"{distanceInMeter:0}m{distanceSymbol}";
        return $"{distanceInMeter / 1000:0.0}km{distanceSymbol}";
    }
}