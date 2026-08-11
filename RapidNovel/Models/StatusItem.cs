using RapidNovel.Models.Enums;

namespace RapidNovel.Models;

/// <summary>
/// A single status message queued for display in the main window status bar.
/// </summary>
public record StatusItem(string Message, StatusSeverity Severity = StatusSeverity.Info);
