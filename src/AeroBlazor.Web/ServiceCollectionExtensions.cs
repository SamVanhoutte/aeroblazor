using AeroBlazor.Configuration;
using AeroBlazor.Security;
using AeroBlazor.Services;
using AeroBlazor.Services.Maps;
using AeroBlazor.Theming;
using AeroBlazor.Web.Configuration;
using AeroBlazor.Web.Identity;
using AeroBlazor.Web.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using MudExtensions.Services;
using MapOptions = AeroBlazor.Configuration.MapOptions;

namespace AeroBlazor.Web;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAeroBlazorWebServices<TThemeManager>(this IServiceCollection services,
        Action<AeroStartupOptions>? configureRuntime = null) where TThemeManager : class, IThemeManager
    {
        services.AddHttpContextAccessor();
        services.AddAeroBlazorServices<TThemeManager>(configureRuntime);
        if (configureRuntime != null)
        {
            ConfigureOptions(services, configureRuntime);
        }
        return services;
    }

    public static IServiceCollection AddAeroAppServices(this IServiceCollection services,
        Action<AeroStartupOptions>? configureRuntime = null)
    {
        services.AddAeroAppServices<StandardLanguage>(configureRuntime);
        return services;
    }
    
    public static IServiceCollection AddAeroAppServices<TLanguageResources>(this IServiceCollection services,
        Action<AeroStartupOptions>? configureRuntime = null)
    {
        services.AddLocalization(options => { options.ResourcesPath = "Resources"; });
        services.AddHttpContextAccessor();
        services.AddScoped<IClipboardService, WebClipboardService>();
        services.AddScoped<ICrashReportHandler, EmptyCrashReportHandler>();
        services.AddScoped<IAlertNotifier, WebAlertNotifier>();
        if (configureRuntime != null)
        {
            ConfigureOptions(services, configureRuntime);
        }

        return services;
    }

    private static AeroStartupOptions ConfigureOptions(IServiceCollection services,
        Action<AeroWebOptions> configureRuntime)
    {
        var options = AeroWebOptions.Default;
        configureRuntime(options);
        if(options.AuthenticationType!=null)
        {
            switch (options.AuthenticationType.Value)
            {
                case AuthenticationType.Auth0:
                    services.AddScoped<IUserProfileService, Auth0UserProfileService>();
                    break;
                case AuthenticationType.EntraId:
                    services.Configure<MicrosoftIdentityOptions>(o =>
                    {
                        o.Domain = options.IdentityOptions!.Domain;
                        o.Instance = options.IdentityOptions!.Instance;
                        o.TenantId = options.IdentityOptions!.TenantId;
                        o.ClientSecret = options.IdentityOptions!.ClientSecret;
                        o.ClientId = options.IdentityOptions!.ClientId;
                        o.SignUpSignInPolicyId = options.IdentityOptions!.SignUpSignInPolicyId;
                    });
            
                    services.AddSingleton<IAuthenticationManager, AzureB2CTokenManager>();
                    if (options.PersistAuthenticationInTableStorage)
                    {
                        if (string.IsNullOrEmpty(options?.TableStorageConfiguration?.StorageAccount))
                        {
                            throw new NullReferenceException("The TableStorage Configuration options are empty");
                        }

                        services.Configure<TableStorageOptions>(o =>
                        {
                            o.StorageAccount = options.TableStorageConfiguration!.StorageAccount;
                            o.ClientName = options.TableStorageConfiguration!.ClientName;
                            o.StorageAccountKey = options.TableStorageConfiguration!.StorageAccountKey;
                            o.AuthTableName = options.TableStorageConfiguration!.AuthTableName;
                        });
                        services.AddSingleton<ITokenStorageProvider, TableStorageProviderTokenProvider>();
                    }

                    if (options.PersistAuthenticationLocally ?? false)
                    {
                        services.AddSingleton<ITokenStorageProvider, ProtectedLocalStorageProviderTokenStorageProvider>();
                    }

                    break;
                default:
                    break;
            }
        }

        services.AddSingleton<AeroStartupOptions>(options);
        return options;
    }
}