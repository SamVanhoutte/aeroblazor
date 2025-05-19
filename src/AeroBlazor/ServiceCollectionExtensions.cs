using AeroBlazor.Configuration;
using AeroBlazor.Services;
using AeroBlazor.Services.Maps;
using AeroBlazor.Theming;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using MudExtensions.Services;
using MapOptions = AeroBlazor.Configuration.MapOptions;

namespace AeroBlazor;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAeroBlazorServices<TThemeManager>(this IServiceCollection services,
        Action<AeroStartupOptions>? configureRuntime = null) where TThemeManager : class, IThemeManager
    {
        services.AddLocalization(options => { options.ResourcesPath = "Languages"; });
        services.AddScoped<IClipboardService, WebClipboardService>();
        services.AddScoped<ICrashReportHandler, EmptyCrashReportHandler>();
        services.AddScoped<IAlertNotifier, WebAlertNotifier>();
        services.AddScoped<IThemeManager, TThemeManager>();
        ConfigureOptions(services, configureRuntime);

        return services;
    }

    public static IServiceCollection AddAeroAppServices<TThemeManager>(this IServiceCollection services,
        Action<AeroStartupOptions>? configureRuntime = null) where TThemeManager : class, IThemeManager
    {
        services.AddLocalization(options => { options.ResourcesPath = "Resources"; });
        services.AddScoped<IClipboardService, WebClipboardService>();
        services.AddScoped<ICrashReportHandler, EmptyCrashReportHandler>();
        services.AddScoped<IAlertNotifier, WebAlertNotifier>();
        services.AddScoped<IThemeManager, TThemeManager>();
        ConfigureOptions(services, configureRuntime);

        return services;
    }

    private static AeroStartupOptions ConfigureOptions(IServiceCollection services,
        Action<AeroStartupOptions> configureRuntime)
    {
        var options = AeroStartupOptions.Default;
        if (configureRuntime != null)
        {
            configureRuntime(options);
        }

        if (options.InjectHttpClient)
        {
            services.AddHttpClient();
        }

        services.AddMudExtensions();
        services.AddSingleton<Localizer>();
        
        if (options.EnableGoogleMaps)
        {
            services.Configure<MapOptions>(o => { o.GoogleMapKey = options.GoogleMapsConfiguration!.GoogleMapKey; });
            services.AddScoped<AeroMapService, AeroMapService>();
        }

        if (options.EnableLocationServices)
        {
            services.AddScoped<ILocationService, WebLocationService>();
        }

        // Register the composite localizer
        services.AddSingleton<IStringLocalizer>(sp =>
        {
            var factory = sp.GetRequiredService<IStringLocalizerFactory>();
            var localizer = new AeroStringLocalizer(factory);
            if (!string.IsNullOrEmpty(options.LanguageResourceName))
            {
                localizer.AddLocalizer(options.LanguageResourceName, options.LanguageAssemblyName);
            }

            return localizer;
        });

        var behaviorOptions = options.BehaviorOptions ?? AeroBehaviorOptions.Default;
        services.Configure<AeroBehaviorOptions>(o => o = behaviorOptions);
        return options;
    }
}