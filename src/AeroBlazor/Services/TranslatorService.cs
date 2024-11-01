using System.Globalization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace AeroBlazor.Services;

public class TranslatorService
{
    private readonly IStringLocalizer<StandardLanguage> sharedLocalizer;
    private readonly ILogger<TranslatorService> logger;

    public TranslatorService(
        IStringLocalizer<StandardLanguage> sharedLocalizer, ILogger<TranslatorService> logger)
    {
        this.sharedLocalizer = sharedLocalizer;
        this.logger = logger;
    }

    public string this[string lookupValue, string? replacement = null]
    {
        get
        {
            var localizedString = sharedLocalizer[lookupValue];
            if (localizedString.ResourceNotFound)
            {
                if (!string.IsNullOrEmpty(replacement))
                {
                    return replacement;
                }
                logger.LogWarning(
                    $"No translation found for {lookupValue} in the SharedResources with searched location {localizedString.SearchedLocation}");
            }

            return localizedString.Value;
        }
    }

    public string GetTitle([System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
    {
        var page = GetPageName(sourceFilePath);
        return this[$"{page}/Title"];
    }
    
    public string GetSubTitle([System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
    {
        var page = GetPageName(sourceFilePath);
        return this[$"{page}/Subtitle"];
    }

    private string GetPageName(string sourceFilePath)
    {
        return sourceFilePath.Split('/').Last().Replace(".razor", "");
    }

    public string? Localize(IDictionary<string, string>? translations)
    {
        if (translations == null) return null;
        if (!translations?.Any() ?? false) return null;
        var currentLanguage = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
        // Try with current language
        if (translations.TryGetValue(currentLanguage, out var localValue))
        {
            return localValue;
        }
        // Backup English
        if (translations.TryGetValue("en", out var enValue))
        {
            return enValue;
        }

        return translations.First().Value;
    }
}