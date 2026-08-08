namespace RapidNovel.Models.Interfaces;

public interface IProjectService
{
    Project? Project { get; set; }

    void SaveProject(string filename);
    void LoadProject(string filename);
}