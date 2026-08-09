using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using RapidNovel.ViewModels;

namespace RapidNovel.Services.Navigation;

/// <summary>
/// Tracks the page currently shown in the main content area and navigates
/// by resolving page view models from the DI container on demand.
/// </summary>
public interface INavigationService
{
    /// <summary>The page currently displayed in the main content area.</summary>
    ViewModelBase CurrentPage { get; }

    /// <summary>
    /// Navigates to the page for <typeparamref name="T"/>.
    /// The view model is resolved from DI, so it may take any constructor dependencies
    /// without the window view model knowing about them.
    /// </summary>
    void NavigateTo<T>() where T : ViewModelBase;
}

public partial class NavigationService : ObservableObject, INavigationService
{
    private readonly IServiceProvider _services;

    [ObservableProperty]
    private ViewModelBase _currentPage;

    public NavigationService(IServiceProvider services)
    {
        _services = services;
        _currentPage = services.GetRequiredService<MainPageViewModel>();
    }

    public void NavigateTo<T>() where T : ViewModelBase
    {
        CurrentPage = _services.GetRequiredService<T>();
    }
}
