using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using RapidNovel.Models.Interfaces;
using RapidNovel.Services;
using RapidNovel.Services.DI;
using RapidNovel.Services.Navigation;
using RapidNovel.ViewModels;
using RapidNovel.Views;

namespace RapidNovel;

public partial class App : Application
{
    private ServiceProvider? _services;
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Register all the services needed for the application to run.
        var collection = new ServiceCollection();
        collection.AddCommonServices();

        // Build the provider used to resolve services.
        _services = collection.BuildServiceProvider();

        // Initialize config file and directories
        _services.GetRequiredService<IConfigService>().Initialize();

        // Initialize navigation service — MUST be done after DI construction completes
        // to avoid circular dependencies (NavigationService ↔ MainPageViewModel ↔ CreateProjectCommand)
        _services.GetRequiredService<INavigationService>().Initialize(_services!);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownRequested += (_, _) => _services?.Dispose();

            desktop.MainWindow = new MainWindow
            {
                DataContext = _services.GetRequiredService<MainWindowViewModel>(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
