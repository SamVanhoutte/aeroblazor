using AeroBlazor.Configuration;
using Microsoft.Identity.Web;

namespace AeroBlazor.Web.Configuration;

public class AeroWebOptions : AeroStartupOptions
{
    internal MicrosoftIdentityOptions IdentityOptions;
    public static AeroWebOptions Default => new AeroWebOptions();
    
    internal bool? PersistAuthenticationLocally;
    internal bool PersistAuthenticationInTableStorage => EnableAuthentication && base.TableStorageConfiguration != null;
    internal bool EnableAuthentication = false;
    
    public void ConfigureAzureB2CAuthentication(MicrosoftIdentityOptions identityOptions, bool? localStorageConfiguration = null,
        TableStorageOptions? tableStorageConfiguration = null)
    {
        EnableAuthentication = tableStorageConfiguration != null || localStorageConfiguration != null;
        TableStorageConfiguration = tableStorageConfiguration;
        PersistAuthenticationLocally = localStorageConfiguration ?? false;
        IdentityOptions = identityOptions;
    }
}