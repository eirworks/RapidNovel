namespace RapidNovel.Models;

public class Project
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;

    public ProjectDb Database { get; set; } = new();
    public ProjectContent Content { get; set; } = new();
    public ProjectManager Manager { get; set; } = new();
}