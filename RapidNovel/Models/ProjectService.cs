using System;
using RapidNovel.Models.Interfaces;
using RapidNovel.Services.Saves;

namespace RapidNovel.Models;

public class ProjectService : IProjectService
{
    private readonly ProjectSaveService _saveService;

    /// <summary>Raised whenever <see cref="Project"/> is replaced with a new instance.</summary>
    public event EventHandler? ProjectChanged;

    private Project? _project;

    /// <summary>Design-time / previewer support.</summary>
    public ProjectService()
        : this(new ProjectSaveService())
    {
    }

    public ProjectService(ProjectSaveService saveService)
    {
        _saveService = saveService;
    }

    /// <summary>The currently open project, kept in memory. <c>null</c> when none is loaded.</summary>
    public Project? Project
    {
        get => _project;
        set
        {
            if (ReferenceEquals(_project, value)) return;
            _project = value;
            ProjectChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Persists the currently open project to <c>~/.rapidnovel/projects/&lt;id&gt;/&lt;id&gt;.rnp</c>.
    /// </summary>
    /// <param name="id">Project id used as save key.</param>
    /// <exception cref="InvalidOperationException">When no project is currently open.</exception>
    public void SaveProject(string id)
    {
        if (Project is null)
        {
            throw new InvalidOperationException("No project is currently open.");
        }

        _saveService.StoreProject(id, Project);
    }

    /// <summary>
    /// Loads the project with the given <paramref name="id"/> from disk and makes it current.
    /// When no such project exists, the currently open project is left untouched.
    /// </summary>
    /// <param name="id">Project id used as save key.</param>
    public void LoadProject(string id)
    {
        var project = _saveService.LoadProject(id);
        if (project is null)
        {
            return;
        }

        Project = project;
    }
}
