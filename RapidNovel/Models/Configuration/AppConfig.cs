using System.Collections.Generic;

namespace RapidNovel.Models.Configuration;

public record AppConfig(
    string? LastQuickWriteDir,
    List<AiProviderConfig> AiProviders
    )
{
    /// <summary>
    /// Default configuration used before any file is loaded or written.
    /// </summary>
    public static AppConfig Default => new(null, []);
}