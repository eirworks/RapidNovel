using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using RapidNovel.Models.Configuration;
using RapidNovel.Models.Interfaces;

namespace RapidNovel.Services.Config;

public class ConfigService : IConfigService
{
    private static readonly string ConfigPath = Path.Combine(AppPaths.ConfigDir, "config.json");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
    };

    public AppConfig AppConfig { get; set; } = AppConfig.Default;

    /// <summary>
    /// Creates <see cref="AppPaths.BaseDir"/> (with 'config' and 'projects' subdirectories)
    /// if it does not exist, then writes a default config.json into 'config' when missing
    /// and loads it.
    /// </summary>
    public void Initialize()
    {
        Directory.CreateDirectory(AppPaths.BaseDir);
        Directory.CreateDirectory(AppPaths.ConfigDir);
        Directory.CreateDirectory(AppPaths.ProjectsDir);

        if (!File.Exists(ConfigPath))
        {
            StoreConfig();
        }

        LoadConfig();
    }

    public void StoreConfig()
    {
        var json = JsonSerializer.Serialize(AppConfig, JsonOptions);
        File.WriteAllText(ConfigPath, json);
    }

    public void LoadConfig()
    {
        if (!File.Exists(ConfigPath))
        {
            AppConfig = AppConfig.Default;
            return;
        }

        var json = File.ReadAllText(ConfigPath);
        var loaded = JsonSerializer.Deserialize<AppConfig>(json, JsonOptions) ?? AppConfig.Default;
        AppConfig = loaded with { AiProviders = loaded.AiProviders ?? [] };
    }
}
