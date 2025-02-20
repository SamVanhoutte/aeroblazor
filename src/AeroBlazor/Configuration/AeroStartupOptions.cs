using System.Reflection;

namespace AeroBlazor.Configuration;

public class AeroStartupOptions
{
    public static AeroStartupOptions Default => new AeroStartupOptions();
    
    public TableStorageOptions? TableStorageConfiguration;
    public MapOptions? GoogleMapsConfiguration;
    public bool EnableGoogleMaps = false;
    public bool InjectHttpClient { get; set; } = true;
    public bool EnableLocationServices { get; set; } = true;
    public string? LanguageResourceName { get; set; }
    public string? LanguageAssemblyName { get; set; }
    public AeroBehaviorOptions? BehaviorOptions { get; set; }

    public void ConfigureMaps(MapOptions? options)
    {
        EnableGoogleMaps = options != null;
        GoogleMapsConfiguration = options;
    }
    
    public void SetLanguage(string languageResourceName, Assembly? assembly = null)
    {
        LanguageResourceName = languageResourceName;
        LanguageAssemblyName = assembly?.FullName;
    }

}