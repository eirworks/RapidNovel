using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;
using RapidNovel.Models.Interfaces;
using RapidNovel.Services.Navigation;
using RapidNovel.ViewModels;
using RapidNovel.Views;

namespace RapidNovel.Commands;

public class CreateProjectCommand : ICommand
{
    private readonly IServiceProvider _services;

    public CreateProjectCommand(IServiceProvider services)
    {
        _services = services;
    }

    public bool CanExecute(object? parameter) => true;

    public async void Execute(object? parameter)
    {
        // Resolve a fresh view model per dialog so state never leaks between opens.
        var viewModel = _services.GetRequiredService<CreateProjectViewModel>();
        var createProjectWindow = new CreateProjectWindow
        {
            DataContext = viewModel,
        };

        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;
        var mainWindow = desktop.MainWindow;

        if (mainWindow is null) return;

        // Remember the current project so we can tell whether the dialog actually
        // created a new project (as opposed to being cancelled).
        var projectService = _services.GetRequiredService<IProjectService>();
        var projectBefore = projectService.Project;

        await createProjectWindow.ShowDialog(mainWindow);

        // The dialog stored a new project — navigate back to the home page so the
        // user lands on the project dashboard. Skip the navigation when the home
        // page is already showing; it stays in sync via ProjectChanged.
        if (!ReferenceEquals(projectBefore, projectService.Project))
        {
            var navigation = _services.GetRequiredService<INavigationService>();
            if (navigation.CurrentPage is not MainPageViewModel)
            {
                navigation.NavigateTo<MainPageViewModel>();
            }
        }
    }

    public event EventHandler? CanExecuteChanged;

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
