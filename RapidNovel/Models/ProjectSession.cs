using RapidNovel.Models.Interfaces;

namespace RapidNovel.Models;

public class ProjectSession:IProjectSession
{
    public required Project Project { get; set; }
    public void SaveProject(string filename)
    {
        throw new System.NotImplementedException();
    }

    public void LoadProject(string filename)
    {
        throw new System.NotImplementedException();
    }
}