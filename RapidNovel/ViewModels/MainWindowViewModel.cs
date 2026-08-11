using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RapidNovel.Commands;
using RapidNovel.Models;
using RapidNovel.Models.Interfaces;
using RapidNovel.Services.Navigation;
using RapidNovel.Services.Status;

namespace RapidNovel.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IProjectService _projectService;
    
    public CreateProjectCommand CreateProjectCmd { get; }

    /// <summary>Persists the currently open project to disk.</summary>
    public SaveProjectCommand SaveProjectCmd { get; }

    /// <summary>The currently loaded project, or <c>null</c> when no project is loaded.</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsProjectLoaded))]
    private Project? _project;

    public bool IsProjectLoaded => Project is not null;

    /// <summary>Navigation state for the main content area.</summary>
    public INavigationService Navigation { get; }

    /// <summary>Global status queue consumed by the status bar at the bottom of the window.</summary>
    public IStatusService Status { get; }

    /// <summary>Navigates to the Characters page.</summary>
    [RelayCommand]
    private void OpenCharacters() => Navigation.NavigateTo<CharactersPageViewModel>();

    public MainWindowViewModel(
        IProjectService projectService,
        INavigationService navigation,
        IStatusService status,
        CreateProjectCommand createProjectCommand)
    {
        _projectService = projectService;
        _project = _projectService.Project;
        // Keep the window in sync when a project is created/loaded elsewhere
        // (e.g. the Create Project dialog stores a new project in the service).
        _projectService.ProjectChanged += (_, _) => Project = _projectService.Project;
        Navigation = navigation;
        Status = status;
        CreateProjectCmd = createProjectCommand;
        SaveProjectCmd = new SaveProjectCommand(projectService, Status);
    }
}
