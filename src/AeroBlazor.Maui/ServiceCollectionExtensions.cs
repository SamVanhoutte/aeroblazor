using AeroBlazor.Configuration;
using AeroBlazor.Services;
using AeroBlazor.Services.Maps;
using AeroBlazor.Theming;
using MudExtensions.Services;
using MapOptions = AeroBlazor.Configuration.MapOptions;

namespace AeroBlazor.Maui;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAeroAppServices<TThemeManager>(this IServiceCollection services,
        Action<AeroStartupOptions>? configureRuntime = null) where TThemeManager : class, IThemeManager
    {
        services.AddLocalization(options => { options.ResourcesPath = "Resources"; });
        services.AddScoped<IClipboardService, WebClipboardService>();
        services.AddScoped<ICrashReportHandler, EmptyCrashReportHandler>();
        services.AddScoped<IAlertNotifier, WebAlertNotifier>();
        services.AddScoped<IThemeManager, TThemeManager>();
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