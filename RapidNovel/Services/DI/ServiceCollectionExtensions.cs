using Microsoft.Extensions.DependencyInjection;
using RapidNovel.Models;
using RapidNovel.Models.Interfaces;
using RapidNovel.Services.Config;
using RapidNovel.ViewModels;

namespace RapidNovel.Services.DI;

/// <summary>
/// Central place to register application services with Microsoft.Extensions.DependencyInjection.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection services)
    {
        // project session
        services.AddSingleton<IProjectSession, ProjectSession>();
        // config service
        services.AddSingleton<IConfigService, ConfigService>();
        // ViewModels
        services.AddSingleton<MainWindowViewModel>();
    }
}
