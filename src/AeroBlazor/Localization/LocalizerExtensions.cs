// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Localization;
//
// namespace AeroBlazor.Localization;
//
// public static class LocalizerExtensions
// {
//     public static IServiceCollection AddInheritStringLocalizerFactory(this IServiceCollection services, Action<LocalizationOptions> action = null)
//     {
//         if (services == null)
//         {
//             throw new ArgumentNullException(nameof(services));
//         }
//
//         services.AddOptions();
//         services.AddSingleton<IStringLocalizerFactory, InheritStringLocalizerFactory>();
//         services.AddTransient(typeof (IStringLocalizer<>), typeof (StringLocalizer<>));
//         if (action != null)
//         {
//             services.Configure(action);
//         }
//
//         return services;
//     }
// }