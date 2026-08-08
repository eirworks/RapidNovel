namespace RapidNovel.Models.Interfaces;

public interface IProjectSession
{
    Project? Project { get; set; }

    void SaveProject(string filename);
    void LoadProject(string filename);
}