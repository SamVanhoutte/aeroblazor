using System.Globalization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace AeroBlazor.Localization;

public class MultiStringLocalizer : IStringLocalizer
{
    private readonly List<IStringLocalizer> m_localizers;
    private readonly ILogger<MultiStringLocalizer> m_logger;
    private readonly CultureInfo m_cultureInfo;

    public MultiStringLocalizer(List<IStringLocalizer> localizers, ILogger<MultiStringLocalizer> logger)
    {
        if (localizers == null)
            throw new ArgumentNullException(nameof(localizers));
        if (localizers.Count == 0)
            throw new ArgumentException("Empty not supported", nameof(localizers));
        m_localizers = localizers;
        m_logger = logger ?? NullLogger<MultiStringLocalizer>.Instance;
    }

    private MultiStringLocalizer(List<IStringLocalizer> localizers, ILogger<MultiStringLocalizer> logger,
        CultureInfo cultureInfo)
        : this(localizers.ToList(), logger)
    {
        m_cultureInfo = cultureInfo;
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        var result = new Dictionary<string, LocalizedString>();
        foreach (var localizer in m_localizers)
        {
            foreach (var entry in localizer.GetAllStrings(includeParentCultures))
            {
                if (!result.ContainsKey(entry.Name))
                {
                    result.Add(entry.Name, entry);
                }
            }
        }

        return result.Values;
    }

    public IStringLocalizer WithCulture(CultureInfo culture)
    {
        if (culture == null)
        {
            return new MultiStringLocalizer(m_localizers.ToList(), m_logger);
        }

        return new MultiStringLocalizer(m_localizers, m_logger, culture);
    }

    private void OnLogAttempt(IStringLocalizer localizer, LocalizedString result, CultureInfo cultureInfo)
    {
        var keyCulture = cultureInfo ?? CultureInfo.CurrentUICulture;
        if (!result.ResourceNotFound)
        {
            m_logger.LogDebug(
                $"{localizer.GetType()} found '{result.Name}' in '{result.SearchedLocation}' with culture '{keyCulture}'");
        }
        else
        {
            m_logger.LogDebug(
                $"{localizer.GetType()} searched for '{result.Name}' in '{result.SearchedLocation}' with culture '{keyCulture}'");
        }
    }

    public LocalizedString this[string name]
    {
        get
        {
            LocalizedString s = null;
            foreach (var localizer in m_localizers)
            {
                s = localizer[name];
                if (!s.ResourceNotFound)
                {
                    OnLogAttempt(localizer, s, m_cultureInfo);
                    return s;
                }

                OnLogAttempt(localizer, s, m_cultureInfo);
            }

            return s;
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            LocalizedString s = null;
            foreach (var localizer in m_localizers)
            {
                s = localizer[name, arguments];
                if (!s.ResourceNotFound)
                {
                    OnLogAttempt(localizer, s, m_cultureInfo);
                    return s;
                }

                OnLogAttempt(localizer, s, m_cultureInfo);
            }

            return s;
        }
    }
}