using System;
using RapidNovel.Models.Interfaces;

namespace RapidNovel.Models;

public class ProjectService : IProjectService
{
    /// <summary>Raised whenever <see cref="Project"/> is replaced with a new instance.</summary>
    public event EventHandler? ProjectChanged;

    private Project? _project;

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

    public void SaveProject(string filename)
    {
        throw new NotImplementedException();
    }

    public void LoadProject(string filename)
    {
        throw new NotImplementedException();
    }
}
