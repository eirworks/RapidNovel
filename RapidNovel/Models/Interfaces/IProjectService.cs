using System;

namespace RapidNovel.Models.Interfaces;

public interface IProjectService
{
    Project? Project { get; set; }

    /// <summary>Raised whenever <see cref="Project"/> is replaced with a new instance.</summary>
    event EventHandler? ProjectChanged;

    void SaveProject(string id);
    void LoadProject(string id);
}
