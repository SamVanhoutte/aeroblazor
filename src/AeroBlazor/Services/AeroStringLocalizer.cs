using System.Reflection;
using Microsoft.Extensions.Localization;

namespace AeroBlazor.Services;

public class AeroStringLocalizer : IStringLocalizer
{
    private readonly IStringLocalizerFactory factory;
    private readonly List<IStringLocalizer> localizers = new();

    public AeroStringLocalizer(IStringLocalizerFactory factory)
    {
        this.factory = factory;
        localizers.Add(new StringLocalizer<StandardLanguage>(factory));
    }

    public void AddLocalizer(string resourceName, string? assemblyName = null)
    {
        assemblyName ??= Assembly.GetCallingAssembly().FullName;
        localizers.Add(factory.Create(resourceName, assemblyName));
    }
    
    public LocalizedString this[string name]
    {
        get
        {
            LocalizedString? lValue = null;
            foreach (var localizer in localizers)
            {
                lValue = localizer[name];
                if (!lValue.ResourceNotFound)
                {
                    return lValue;
                }
            }

            return lValue!;
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            LocalizedString? lValue = null;
            foreach (var localizer in localizers)
            {
                lValue = localizer[name, arguments];
                if (!lValue.ResourceNotFound)
                {
                    return lValue;
                }
            }

            return lValue!;
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        var strings = new List<LocalizedString>();
        foreach (var localizer in localizers)
        {
            strings.AddRange(localizer.GetAllStrings(includeParentCultures));
        }

        return strings
            .GroupBy(s => s.Name)
            .Select(g => g.First());
    }
}