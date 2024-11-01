using Microsoft.Identity.Web;

namespace AeroBlazor.Configuration;

public class AeroStartupOptions
{
    public static AeroStartupOptions Default => new AeroStartupOptions();
    
    internal TableStorageOptions? TableStorageConfiguration;
    internal MapOptions? GoogleMapsConfiguration;
    internal bool? PersistAuthenticationLocally;
    internal bool PersistAuthenticationInTableStorage => EnableAuthentication && TableStorageConfiguration != null;
    internal bool EnableAuthentication = false;
    internal MicrosoftIdentityOptions IdentityOptions;
    internal bool EnableGoogleMaps = false;
    public bool InjectHttpClient { get; set; } = true;
    public bool EnableLocationServices { get; set; } = true;
    public AeroBehaviorOptions? BehaviorOptions { get; set; }

    public void ConfigureMaps(MapOptions? options)
    {
        EnableGoogleMaps = options != null;
        GoogleMapsConfiguration = options;
    }
    public void ConfigureAzureB2CAuthentication(MicrosoftIdentityOptions identityOptions, bool? localStorageConfiguration = null,
        TableStorageOptions? tableStorageConfiguration = null)
    {
        EnableAuthentication = tableStorageConfiguration != null || localStorageConfiguration != null;
        TableStorageConfiguration = tableStorageConfiguration;
        PersistAuthenticationLocally = localStorageConfiguration ?? false;
        IdentityOptions = identityOptions;
    }
}