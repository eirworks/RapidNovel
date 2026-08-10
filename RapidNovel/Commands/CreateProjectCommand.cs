using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;
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

    public void Execute(object? parameter)
    {
        // Resolve a fresh view model per dialog so state never leaks between opens.
        var viewModel = _services.GetRequiredService<CreateProjectViewModel>();
        var createProjectWindow = new CreateProjectWindow
        {
            DataContext = viewModel,
        };

        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;
        var mainWindow = desktop.MainWindow;

        if (mainWindow is not null)
        {
            createProjectWindow.ShowDialog(mainWindow);
        }
    }

    public event EventHandler? CanExecuteChanged;

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
