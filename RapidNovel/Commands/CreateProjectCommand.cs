using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using RapidNovel.Models.Interfaces;
using RapidNovel.Services.Navigation;
using RapidNovel.ViewModels;
using RapidNovel.Views;

namespace RapidNovel.Commands;

public class CreateProjectCommand(
    INavigationService navigation)
    : ICommand
{
    private readonly INavigationService _navigation = navigation;

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter)
    {
        var createProjectWindow = new CreateProjectWindow();
        
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