using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RapidNovel.Models;
using RapidNovel.Models.Interfaces;

namespace RapidNovel.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Greeting { get; } = "Welcome to Avalonia!";

    private readonly IProjectSession _projectSession;

    /// <summary>The currently loaded project, or <c>null</c> when no project is loaded.</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsProjectLoaded))]
    private Project? _project;

    public bool IsProjectLoaded => Project is not null;

    public MainWindowViewModel(IProjectSession projectSession)
    {
        _projectSession = projectSession;
        _project = _projectSession.Project;
    }
}
