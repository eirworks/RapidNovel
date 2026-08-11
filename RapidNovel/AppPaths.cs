using System;
using System.IO;

namespace RapidNovel;

/// <summary>
/// Well-known application data directories under the user profile
/// (<c>~/.rapidnovel</c>), shared by services that read/write persistent app data.
/// </summary>
public static class AppPaths
{
    /// <summary>Root data directory: <c>~/.rapidnovel</c>.</summary>
    public static readonly string BaseDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".rapidnovel");

    /// <summary>Config file directory: <c>~/.rapidnovel/config</c>.</summary>
    public static readonly string ConfigDir = Path.Combine(BaseDir, "config");

    /// <summary>Project save directory: <c>~/.rapidnovel/projects</c>.</summary>
    public static readonly string ProjectsDir = Path.Combine(BaseDir, "projects");
}
