using System.Collections.Generic;
using RapidNovel.Models;

namespace RapidNovel.Services.Saves;

public class ProjectSaveService
{
    /// <summary>
    /// List all project files
    /// </summary>
    /// <returns></returns>
    public List<string> GetProjects()
    {
        // TODO list all .bin files
        return [];
    }

    /// <summary>
    /// Store project
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="project"></param>
    public void StoreProject(string filename, Project project)
    {
        
    }

    /// <summary>
    /// Load project from given filename.
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    public Project? LoadProject(string filename)
    {
        return null;
    }
}