using System;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RapidNovel.Models;
using RapidNovel.Models.Interfaces;

namespace RapidNovel.ViewModels;

/// <summary>
/// View model for the "Create New Project" dialog.
/// Collects the project name and author, builds a <see cref="Project"/> with a
/// generated id and hands it to the <see cref="IProjectService"/> (in-memory).
/// </summary>
public partial class CreateProjectViewModel : ViewModelBase
{
    private readonly IProjectService _projectService;

    /// <summary>Design-time / previewer support.</summary>
    public CreateProjectViewModel()
        : this(new ProjectService())
    {
    }

    public CreateProjectViewModel(IProjectService projectService)
    {
        _projectService = projectService;
    }

    /// <summary>Project name — required before the project can be created.</summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateCommand))]
    [NotifyPropertyChangedFor(nameof(ValidationMessage))]
    private string _name = string.Empty;

    /// <summary>Author / pen name of the project.</summary>
    [ObservableProperty]
    private string _author = string.Empty;

    /// <summary>Inline validation hint shown while the name is empty.</summary>
    public string ValidationMessage =>
        string.IsNullOrWhiteSpace(Name) ? "Please enter a project name." : string.Empty;

    private bool CanCreate() => !string.IsNullOrWhiteSpace(Name);

    /// <summary>
    /// Builds a new <see cref="Project"/>, stores it in the in-memory project service
    /// and closes the dialog.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanCreate))]
    private void Create(Window? window)
    {
        var project = new Project
        {
            Id = Guid.NewGuid().ToString("N"),
            Name = Name.Trim(),
            Author = Author.Trim(),
        };

        _projectService.Project = project;
        window?.Close();
    }

    /// <summary>Closes the dialog without creating a project.</summary>
    [RelayCommand]
    private void Cancel(Window? window) => window?.Close();
}
