using Microsoft.Extensions.DependencyInjection;
using RapidNovel.Commands;
using RapidNovel.Models;
using RapidNovel.Models.Interfaces;
using RapidNovel.Services.Config;
using RapidNovel.Services.Navigation;
using RapidNovel.Services.Saves;
using RapidNovel.ViewModels;
using RapidNovel.Views;

namespace RapidNovel.Services.DI;

/// <summary>
/// Central place to register application services with Microsoft.Extensions.DependencyInjection.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection services)
    {
        // project session
        services.AddSingleton<IProjectService, ProjectService>();
        // config service
        services.AddSingleton<IConfigService, ConfigService>();
        // navigation
        services.AddSingleton<INavigationService, NavigationService>();
        // ViewModels
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<MainPageViewModel>();
        services.AddTransient<CreateProjectViewModel>();
        // commands
        services.AddTransient<CreateProjectCommand>();
        
        // other services
        services.AddTransient<ProjectSaveService>();
    }
}
