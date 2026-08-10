using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using RapidNovel.Models;
using RapidNovel.Models.Interfaces;

namespace RapidNovel.Services.Config;

public class ConfigService: IConfigService
{
    private static readonly string BaseDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".rapidnovel");
    
    private static readonly string ConfigDir = Path.Combine(BaseDir, "config");
    
    /// <summary>
    /// Directory where project files are stored: <c>~/.rapidnovel/projects</c>.
    /// </summary>
    public static readonly string ProjectsDir = Path.Combine(BaseDir, "projects");
    
    private static readonly string ModelConfigPath = Path.Combine(ConfigDir, "model_config.json");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
    };

    public IModelConfig Config { get; set; } = new ModelConfig();

    /// <summary>
    /// Creates ~/.rapidnovel (with 'config' and 'projects' subdirectories) if it does not exist,
    /// then writes a default model_config.json into 'config' when missing and loads it.
    /// </summary>
    public void Initialize()
    {
        Directory.CreateDirectory(BaseDir);
        Directory.CreateDirectory(ConfigDir);
        Directory.CreateDirectory(ProjectsDir);

        if (!File.Exists(ModelConfigPath))
        {
            StoreConfig();
        }

        LoadConfig();
    }

    public void StoreConfig()
    {
        var json = JsonSerializer.Serialize(Config, JsonOptions);
        File.WriteAllText(ModelConfigPath, json);
    }

    public void LoadConfig()
    {
        if (!File.Exists(ModelConfigPath))
        {
            Config = new ModelConfig();
            return;
        }

        var json = File.ReadAllText(ModelConfigPath);
        Config = JsonSerializer.Deserialize<ModelConfig>(json, JsonOptions) ?? new ModelConfig();
    }
}
