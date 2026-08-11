namespace RapidNovel.Models.Enums;

/// <summary>Severity level of a status message shown in the main window status bar.</summary>
public enum StatusSeverity
{
    /// <summary>Neutral informational message.</summary>
    Info,

    /// <summary>Positive confirmation (e.g. a save or export finished).</summary>
    Success,

    /// <summary>Something needs attention, but is not an error.</summary>
    Warning,

    /// <summary>An operation failed.</summary>
    Error,
}
