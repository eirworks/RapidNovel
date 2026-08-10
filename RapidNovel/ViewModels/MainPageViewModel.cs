using RapidNovel.Commands;

namespace RapidNovel.ViewModels;

public class MainPageViewModel : ViewModelBase
{
    public CreateProjectCommand CreateProjectCommand { get; }
    public string Welcome { get; } = "Welcome to RapidNovel!";
    public string Hint { get; } = "Create a project using the New Project button below.";
    public string NewProjectButton { get; } = "New Project";

    public MainPageViewModel(CreateProjectCommand createProjectCommand)
    {
        CreateProjectCommand = createProjectCommand;
        // inject required singleton/factory 
    }
}
