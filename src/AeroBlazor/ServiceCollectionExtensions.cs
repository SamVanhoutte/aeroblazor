using AeroBlazor.Configuration;
using AeroBlazor.Security;
using AeroBlazor.Services;
using AeroBlazor.Services.Maps;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using MudExtensions.Services;
using MapOptions = AeroBlazor.Configuration.MapOptions;

namespace AeroBlazor;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAeroBlazorWebServices(this IServiceCollection services,
        Action<AeroStartupOptions>? configureRuntime = null)
    {
        services.AddLocalization(options => { options.ResourcesPath = "Resources"; });
        services.AddTransient<TranslatorService>();
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

    public static IServiceCollection AddAeroAppServices(this IServiceCollection services,
        Action<AeroStartupOptions>? configureRuntime = null)
    {
        services.AddLocalization(options => { options.ResourcesPath = "Resources"; });
        services.AddScoped<TranslatorService>();
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
        Action<AeroStartupOptions> configureRuntime)
    {
        var options = AeroStartupOptions.Default;
        configureRuntime(options);
        if (options.InjectHttpClient)
        {
            services.AddHttpClient();
        }

        services.AddMudExtensions();
        if (options.EnableAuthentication)
        {
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
        }

        if (options.EnableGoogleMaps)
        {
            services.Configure<MapOptions>(o =>
            {
                o.GoogleMapKey = options.GoogleMapsConfiguration!.GoogleMapKey;
            });
            services.AddScoped<AeroMapService, AeroMapService>();
        }

        if (options.EnableLocationServices)
        {
            services.AddScoped<ILocationService, WebLocationService>();
        }
        var behaviorOptions = options.BehaviorOptions ?? AeroBehaviorOptions.Default;
        services.Configure<AeroBehaviorOptions>(o => o = behaviorOptions);
        return options;
    }
}