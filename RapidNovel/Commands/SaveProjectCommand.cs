using System;
using System.Windows.Input;
using RapidNovel.Models.Enums;
using RapidNovel.Models.Interfaces;
using RapidNovel.Services.Status;

namespace RapidNovel.Commands;

/// <summary>
/// Persists the currently open project to disk via <see cref="IProjectService.SaveProject"/>.
/// </summary>
public class SaveProjectCommand : ICommand
{
    private readonly IProjectService _projectService;
    private readonly IStatusService _status;

    public SaveProjectCommand(IProjectService projectService, IStatusService status)
    {
        _projectService = projectService;
        _status = status;
    }

    public bool CanExecute(object? parameter) => _projectService.Project is not null;

    public void Execute(object? parameter)
    {
        var project = _projectService.Project;
        if (project is null) return;

        try
        {
            _projectService.SaveProject(project.Id);
            _status.Enqueue($"Project \"{project.Name}\" saved", StatusSeverity.Success);
        }
        catch (Exception ex)
        {
            _status.Enqueue($"Failed to save project: {ex.Message}", StatusSeverity.Error);
        }
    }

    public event EventHandler? CanExecuteChanged;

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
