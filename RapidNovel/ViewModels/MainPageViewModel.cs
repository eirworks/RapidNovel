namespace RapidNovel.ViewModels;

public class MainPageViewModel : ViewModelBase
{
    public string Welcome { get; } = "Welcome to RapidNovel!";
    public string Hint { get; } = "Create a project using the New Project button below.";
    public string NewProjectButton { get; } = "New Project";

    public MainPageViewModel()
    {
        // inject required singleton/factory 
    }
}
