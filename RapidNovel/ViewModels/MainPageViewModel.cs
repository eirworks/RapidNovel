using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RapidNovel.Commands;
using RapidNovel.Models;
using RapidNovel.Models.Interfaces;
using RapidNovel.Services.Navigation;

namespace RapidNovel.ViewModels;

public partial class MainPageViewModel : ViewModelBase
{
    private readonly IProjectService _projectService;
    private readonly INavigationService _navigation;
    private readonly Random _random = new();

    public CreateProjectCommand CreateProjectCommand { get; }
    public string Welcome { get; } = "Welcome to RapidNovel!";
    public string Hint { get; } = "Create a project using the New Project button below.";
    public string NewProjectButton { get; } = "New Project";

    /// <summary>The currently loaded project, or <c>null</c> when no project is loaded.</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsProjectLoaded))]
    private Project? _project;

    /// <summary>True while a project is loaded; drives which content the home page shows.</summary>
    public bool IsProjectLoaded => Project is not null;

    // Placeholder stats — random values until real data sources are wired up.
    public int DraftsCount { get; private set; }
    public int TasksCount { get; private set; }
    public int NotesCount { get; private set; }
    public int CharactersCount { get; private set; }
    public int PlacesCount { get; private set; }
    public int ItemsCount { get; private set; }

    public MainPageViewModel(
        CreateProjectCommand createProjectCommand,
        IProjectService projectService,
        INavigationService navigation)
    {
        CreateProjectCommand = createProjectCommand;
        _projectService = projectService;
        _navigation = navigation;

        _project = _projectService.Project;
        RefreshStats();

        // Keep the page in sync when a project is created/loaded elsewhere
        // (e.g. the Create Project dialog stores a new project in the service).
        _projectService.ProjectChanged += (_, _) =>
        {
            Project = _projectService.Project;
            RefreshStats();
        };
    }

    /// <summary>Navigates to the Settings page.</summary>
    [RelayCommand]
    private void OpenSettings() => _navigation.NavigateTo<SettingsPageViewModel>();

    /// <summary>
    /// Fills the stat cards with random placeholder values.
    /// TODO: replace with real counts once drafts/tasks/notes/characters/places/items exist.
    /// </summary>
    private void RefreshStats()
    {
        DraftsCount = _random.Next(1, 100);
        TasksCount = _random.Next(1, 100);
        NotesCount = _random.Next(1, 100);
        CharactersCount = _random.Next(1, 100);
        PlacesCount = _random.Next(1, 100);
        ItemsCount = _random.Next(1, 100);

        OnPropertyChanged(nameof(DraftsCount));
        OnPropertyChanged(nameof(TasksCount));
        OnPropertyChanged(nameof(NotesCount));
        OnPropertyChanged(nameof(CharactersCount));
        OnPropertyChanged(nameof(PlacesCount));
        OnPropertyChanged(nameof(ItemsCount));
    }
}
