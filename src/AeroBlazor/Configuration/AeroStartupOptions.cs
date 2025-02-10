namespace AeroBlazor.Configuration;

public class AeroStartupOptions
{
    public static AeroStartupOptions Default => new AeroStartupOptions();
    
    public TableStorageOptions? TableStorageConfiguration;
    internal MapOptions? GoogleMapsConfiguration;
    public bool EnableGoogleMaps = false;
    public bool InjectHttpClient { get; set; } = true;
    public bool EnableLocationServices { get; set; } = true;
    public AeroBehaviorOptions? BehaviorOptions { get; set; }

    public void ConfigureMaps(MapOptions? options)
    {
        EnableGoogleMaps = options != null;
        GoogleMapsConfiguration = options;
    }
    
}