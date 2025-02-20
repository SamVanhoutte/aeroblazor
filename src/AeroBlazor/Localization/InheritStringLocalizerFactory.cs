using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AeroBlazor.Localization;

public class InheritStringLocalizerFactory : IStringLocalizerFactory
{
    private readonly ConcurrentDictionary<Type, IStringLocalizer> m_cache = new ConcurrentDictionary<Type, IStringLocalizer>();
    private readonly ResourceManagerStringLocalizerFactory m_factory;
    private readonly ILoggerFactory m_loggerFactory;

    public InheritStringLocalizerFactory(IOptions<LocalizationOptions> localizationOptions, ILoggerFactory loggerFactory)
    {
        m_factory = new ResourceManagerStringLocalizerFactory(localizationOptions, loggerFactory);
        m_loggerFactory = loggerFactory;
    }

    public IStringLocalizer Create(Type resourceSource)
    {
        if (resourceSource == null)
            throw new ArgumentNullException(nameof(resourceSource));

        return CreateStringLocalizer(resourceSource);
    }

    private IStringLocalizer CreateStringLocalizer(Type type)
    {
        return m_cache.GetOrAdd(type, CreateStringLocalizerDirect);
    }

    private IStringLocalizer CreateStringLocalizerDirect(Type t)
    {
        var attributes = t.GetCustomAttributes<InheritStringLocalizationAttribute>();
        if (attributes.Any())
        {
            var localizers = new List<IStringLocalizer>();
            var localizer = m_factory.Create(t);
            localizers.Add(localizer);
            foreach (var attribute in attributes.OrderBy(a => a.Priority))
            {
                localizer = CreateStringLocalizer(attribute.InheritFrom);
                localizers.Add(localizer);
            }

            return new MultiStringLocalizer(localizers, m_loggerFactory.CreateLogger<MultiStringLocalizer>());
        }

        return m_factory.Create(t);
    }

    public IStringLocalizer Create(string baseName, string location) => m_factory.Create(baseName, location);
}