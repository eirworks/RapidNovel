using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RapidNovel.Commands;
using RapidNovel.Models;
using RapidNovel.Models.Interfaces;
using RapidNovel.Services.Navigation;

namespace RapidNovel.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IProjectService _projectService;
    
    public CreateProjectCommand CreateProjectCmd { get; }

    /// <summary>The currently loaded project, or <c>null</c> when no project is loaded.</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsProjectLoaded))]
    private Project? _project;

    public bool IsProjectLoaded => Project is not null;

    /// <summary>Navigation state for the main content area.</summary>
    public INavigationService Navigation { get; }
    
    public MainWindowViewModel(
        IProjectService projectService,
        INavigationService navigation,
        CreateProjectCommand createProjectCommand)
    {
        _projectService = projectService;
        _project = _projectService.Project;
        Navigation = navigation;
        CreateProjectCmd = createProjectCommand;
    }
}
