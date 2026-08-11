using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MessagePack;
using RapidNovel.Models;
using RapidNovel.Services.Config;

namespace RapidNovel.Services.Saves;

public class ProjectSaveService
{
    /// <summary>
    /// Resolver that can serialize/deserialize POCO types without <c>[MessagePackObject]</c>
    /// attributes by matching members by name.
    /// </summary>
    private static readonly MessagePackSerializerOptions Options =
        MessagePackSerializerOptions.Standard
            .WithResolver(MessagePack.Resolvers.ContractlessStandardResolver.Instance);

    /// <summary>
    /// List all stored project ids (directories under <c>~/.rapidnovel/projects</c>
    /// that contain a <c>&lt;id&gt;.rnp</c> save file), sorted case-insensitively.
    /// </summary>
    /// <returns>Project ids; empty list when the projects directory does not exist yet.</returns>
    public List<string> GetProjects()
    {
        if (!Directory.Exists(ConfigService.ProjectsDir))
        {
            return [];
        }

        return Directory.GetDirectories(ConfigService.ProjectsDir)
            .Where(dir => File.Exists(Path.Combine(dir, Path.GetFileName(dir) + ".rnp")))
            .Select(Path.GetFileName)
            .OrderBy(id => id, StringComparer.OrdinalIgnoreCase)
            .ToList()!;
    }

    /// <summary>
    /// Store project to <c>~/.rapidnovel/projects/&lt;id&gt;/&lt;id&gt;.rnp</c>.
    /// The write is atomic (temp file + move) so a crash mid-write cannot corrupt an existing project.
    /// </summary>
    /// <param name="id">Project id; used as both the directory and file name. Path separators are stripped to prevent path traversal.</param>
    /// <param name="project">Project to serialize.</param>
    public void StoreProject(string id, Project project)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Project id must not be empty.", nameof(id));
        }

        ArgumentNullException.ThrowIfNull(project);

        Directory.CreateDirectory(ConfigService.ProjectsDir);

        var path = GetProjectPath(id);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        var tempPath = path + ".tmp";

        var bytes = MessagePackSerializer.Serialize(project, Options);
        File.WriteAllBytes(tempPath, bytes);
        File.Move(tempPath, path, overwrite: true);
    }

    /// <summary>
    /// Load project from <c>~/.rapidnovel/projects/&lt;id&gt;/&lt;id&gt;.rnp</c>.
    /// </summary>
    /// <param name="id">Project id; used as both the directory and file name.</param>
    /// <returns>The deserialized project, or <c>null</c> when the file does not exist.</returns>
    public Project? LoadProject(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        var path = GetProjectPath(id);
        if (!File.Exists(path))
        {
            return null;
        }

        var bytes = File.ReadAllBytes(path);
        return MessagePackSerializer.Deserialize<Project>(bytes, Options);
    }

    /// <summary>
    /// Resolves a project <paramref name="id"/> to its save file path:
    /// <c>~/.rapidnovel/projects/&lt;id&gt;/&lt;id&gt;.rnp</c>.
    /// Uses only the file name (stripping any directory parts) and enforces the <c>.rnp</c> extension.
    /// </summary>
    private static string GetProjectPath(string id)
    {
        var safeName = Path.GetFileName(id);
        if (string.IsNullOrEmpty(safeName))
        {
            throw new ArgumentException($"Project id '{id}' does not resolve to a valid file name.", nameof(id));
        }

        return Path.Combine(ConfigService.ProjectsDir, safeName, safeName + ".rnp");
    }
}
