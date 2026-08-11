using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using RapidNovel.Models.Configuration;
using RapidNovel.Models.Enums;
using RapidNovel.Models.Interfaces;

namespace RapidNovel.Services.Config;

public class ConfigService : IConfigService
{
    private static readonly string BaseDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".rapidnovel");

    private static readonly string ConfigDir = Path.Combine(BaseDir, "config");

    /// <summary>
    /// Directory where project files are stored: <c>~/.rapidnovel/projects</c>.
    /// </summary>
    public static readonly string ProjectsDir = Path.Combine(BaseDir, "projects");

    private static readonly string AppConfigPath = Path.Combine(ConfigDir, "app_config.json");

    /// <summary>
    /// Legacy <c>model_config.json</c> written by the old <c>ModelConfig</c>-based service.
    /// Migrated once to <see cref="AppConfigPath"/> and then deleted.
    /// </summary>
    private static readonly string LegacyModelConfigPath = Path.Combine(ConfigDir, "model_config.json");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
    };

    public AppConfig AppConfig { get; set; } = AppConfig.Default;

    /// <summary>
    /// Creates ~/.rapidnovel (with 'config' and 'projects' subdirectories) if it does not exist,
    /// then writes a default app_config.json into 'config' when missing (migrating the legacy
    /// model_config.json if present) and loads it.
    /// </summary>
    public void Initialize()
    {
        Directory.CreateDirectory(BaseDir);
        Directory.CreateDirectory(ConfigDir);
        Directory.CreateDirectory(ProjectsDir);

        if (!File.Exists(AppConfigPath))
        {
            if (File.Exists(LegacyModelConfigPath))
            {
                MigrateLegacyConfig();
            }
            else
            {
                StoreConfig();
            }
        }

        LoadConfig();
    }

    public void StoreConfig()
    {
        var json = JsonSerializer.Serialize(AppConfig, JsonOptions);
        File.WriteAllText(AppConfigPath, json);
    }

    public void LoadConfig()
    {
        if (!File.Exists(AppConfigPath))
        {
            AppConfig = AppConfig.Default;
            return;
        }

        var json = File.ReadAllText(AppConfigPath);
        var loaded = JsonSerializer.Deserialize<AppConfig>(json, JsonOptions) ?? AppConfig.Default;
        AppConfig = loaded with { AiProviders = loaded.AiProviders ?? [] };
    }

    /// <summary>
    /// Best-effort one-time migration from the old <c>ModelConfig</c> shape
    /// (<c>model_config.json</c>) to the new <c>AppConfig</c> shape.
    /// Each legacy API key becomes an <see cref="AiProviderConfig"/> with a sensible
    /// default <see cref="AiProviderConfig.BaseUrl"/> for its provider.
    /// The legacy file is only removed once the migration succeeds; on failure the
    /// original file is left untouched so no user data is lost.
    /// </summary>
    private void MigrateLegacyConfig()
    {
        try
        {
            var json = File.ReadAllText(LegacyModelConfigPath);
            var legacy = JsonSerializer.Deserialize<LegacyModelConfig>(json, JsonOptions);

            var aiProviders = (legacy?.Keys ?? [])
                .Where(key => key.Provider is not null)
                .Select(key => new AiProviderConfig(key.Provider, GetDefaultBaseUrl(key.Provider!.Value), key.ApiKey ?? string.Empty))
                .ToList();

            AppConfig = new AppConfig(null, aiProviders);
            StoreConfig();
            File.Delete(LegacyModelConfigPath);
        }
        catch (JsonException)
        {
            // Corrupt or unreadable legacy file: keep it for manual inspection and
            // fall back to a fresh configuration.
            AppConfig = AppConfig.Default;
            StoreConfig();
        }
    }

    private static string GetDefaultBaseUrl(AiProvider provider) => provider switch
    {
        AiProvider.OpenRouter => "https://openrouter.ai/api/v1",
        AiProvider.LmStudio => "http://localhost:1234/v1",
        AiProvider.DeepSeek => "https://api.deepseek.com",
        AiProvider.OpenAi => "https://api.openai.com/v1",
        AiProvider.Gemini => "https://generativelanguage.googleapis.com/v1beta",
        _ => string.Empty
    };

    /// <summary>Legacy shape of <c>model_config.json</c>, kept only for migration.</summary>
    private sealed record LegacyModelConfig(
        AiProvider? Provider,
        string? Model,
        List<LegacyConfigModelKey>? Keys);

    /// <summary>Legacy API-key entry, kept only for migration.</summary>
    private sealed record LegacyConfigModelKey(AiProvider? Provider, string? ApiKey);
}
