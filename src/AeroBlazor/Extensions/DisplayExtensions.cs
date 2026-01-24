using System.Globalization;
using AeroBlazor.Services;
using Columbae;
using Microsoft.Extensions.Localization;

namespace AeroBlazor.Extensions;

public static class DisplayExtensions
{
    public static string PrintHistoricalTime(this DateTimeOffset? utcTimestamp, IStringLocalizer? localizer=null)
    {
        if (!utcTimestamp.HasValue) return "-";
        return utcTimestamp.Value.PrintHistoricalTime(localizer);
    }

    public static string PrintHistoricalTime(this DateTimeOffset utcTimestamp, IStringLocalizer? localizer=null)
    {
        // TODO : pick time zone from current browser
        return utcTimestamp.DateTime.PrintHistoricalTime(localizer);
    }

    public static string PrintHistoricalTime(this DateTime utcTimestamp, IStringLocalizer? localizer=null)
    {
        var localTime = TimeZoneInfo.ConvertTime(utcTimestamp, TimeZoneInfo.Local);
        var currentDate = DateTime.UtcNow;
        if (currentDate <= utcTimestamp)
        {
            return Translate(localizer, "time/now", "Now");
        }

        var timeAgo = currentDate - utcTimestamp;
        // If less than a minute
        if (timeAgo < TimeSpan.FromMinutes(1))
        {
            return Translate(localizer, "time/secondsago", "{0} seconds ago", timeAgo.Seconds);
        }

        // If less than an hour
        if (timeAgo < TimeSpan.FromHours(1))
        {
            return Translate(localizer, "time/minutesago", "{0} minutes ago", timeAgo.Minutes);
        }

        // If today
        if (timeAgo < TimeSpan.FromHours(24) && currentDate.Day == utcTimestamp.Day)
        {
            return $"{Translate(localizer, "time/today", "Today")} {localTime:HH:mm}";
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

    public static string PrintHistoricalDate(this DateTime utcTimestamp, IStringLocalizer? translatorService=null)
    {
        // Calculate the difference in days between the input date and today
        var localTime = TimeZoneInfo.ConvertTime(utcTimestamp, TimeZoneInfo.Local);
        int daysDifference = (localTime - DateTime.Today).Days;

        // Determine the description based on the difference in days
        return daysDifference switch
        {
            -1 => Translate(translatorService, "Yesterday", "Yesterday"),
            0 => Translate(translatorService, "Today"),
            1 => Translate(translatorService, "Tomorrow"),
            _ when daysDifference < 0 => $"{Math.Abs(daysDifference)} {Translate(translatorService, "days ago")}",
            _ => $"{string.Format(Translate(translatorService, "in {0} days", value: daysDifference))}"
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

    public static string PrintTime(this int? intervalSeconds)
    {
        return intervalSeconds==null ? "-" : TimeSpan.FromSeconds(intervalSeconds.Value).ToString(@"hh\:mm\:ss");
    }

    
    public static string PrintNumber(this int value, string? suffix = null, bool belowZeroIsNull = false,
        bool addSign = false)
    {
        long v = value;
        return v.PrintNumber(suffix, belowZeroIsNull, addSign);
    }

    public static string PrintNumber(this int? value, string suffix = null, bool belowZeroIsNull = false,bool addSign = false)
    {
        return value == null ? "-" : value.Value.PrintNumber(suffix, belowZeroIsNull, addSign);
    }

    public static string PrintNumber(this long value, string suffix = null, bool belowZeroIsNull = false,bool addSign = false)
    {
        if (belowZeroIsNull && value <= 0) return "-";
        var numberValue = value.ToString("#,###", new NumberFormatInfo {NumberGroupSeparator = "."});
        if (addSign && value != 0)
        {
            var sign = value < 0 ? "" : "+";
            numberValue = sign + numberValue;
        }
        if (!string.IsNullOrEmpty(suffix))
        {
            numberValue += suffix;
        }

        return numberValue;
    }
    
    public static string PrintFileSize(this long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
    
    
    public static string PrintDecimal(this decimal? value, string? suffix = null)
    {
        if (value == null) return "-";
        return value.Value.PrintDecimal(suffix);
        return $"{value:N2}{suffix}";
    }
    
    public static string PrintDecimal(this decimal value, string? suffix = null)
    {
        return $"{value:N2}{suffix}";
    }
    public static string PrintNumber(this double? value, string? suffix = null)
    {
        if (value == null) return "-";
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

    public static string PrintDistance(this double distanceInMeter, IStringLocalizer? translatorService,
        bool includeDistanceSymbol = false)
    {
        var distanceSymbol = includeDistanceSymbol ? "" : " ↔";
        if (distanceInMeter < 0) return "";
        if (distanceInMeter < 50) return $"{Translate(translatorService, "Nearby")}{distanceSymbol}";
        if (distanceInMeter < 1000) return $"{distanceInMeter:0}m{distanceSymbol}";
        return $"{distanceInMeter / 1000:0.0}km{distanceSymbol}";
    }

    private static string Translate(this IStringLocalizer? translatorService, string key, string? defaultValue = null,
        object? value = null)
    {
        defaultValue ??= key;
        if (translatorService == null)
        {
            return value == null ? defaultValue : string.Format(defaultValue, value);
        }

        return value == null ? translatorService[key] : string.Format(translatorService[key], value);
    }
}