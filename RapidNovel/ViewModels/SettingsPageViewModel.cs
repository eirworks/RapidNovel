using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RapidNovel.Models.Configuration;
using RapidNovel.Models.Enums;
using RapidNovel.Models.Interfaces;
using RapidNovel.Services.Config;
using RapidNovel.Services.Status;

namespace RapidNovel.ViewModels;

/// <summary>
/// View model for the Settings page: a tabbed page with an "App" tab (a single
/// <see cref="LastQuickWriteDir"/> string) and an "AI" tab (a sidebar list of
/// <see cref="AiProviderConfig"/> plus an edit form, mirroring the Characters page
/// layout).
///
/// <see cref="AiProviderConfig"/> is an immutable record, so the form edits a
/// mutable working copy (the <c>Provider*</c> properties) and rebuilds a new record
/// on save. Every save persists the whole <see cref="AppConfig"/> through
/// <see cref="IConfigService.StoreConfig"/>.
/// </summary>
public partial class SettingsPageViewModel : ViewModelBase
{
    private readonly IConfigService _configService;
    private readonly IStatusService _status;

    /// <summary>Working copy of the configured AI providers (sidebar source).</summary>
    private readonly ObservableCollection<AiProviderConfig> _providers;

    /// <summary>Design-time / previewer support: seeds a sample config.</summary>
    public SettingsPageViewModel()
        : this(
            new ConfigService
            {
                AppConfig = new AppConfig(
                    @"C:\Users\author\QuickWrite",
                    new List<AiProviderConfig>
                    {
                        new("OpenRouter", AiProvider.OpenRouter, "https://openrouter.ai/api/v1", "sk-or-demo"),
                        new("Local (LM Studio)", AiProvider.LmStudio, "http://localhost:1234/v1", string.Empty),
                        new("DeepSeek", AiProvider.DeepSeek, "https://api.deepseek.com", "sk-ds-demo"),
                    }),
            },
            new StatusService())
    {
    }

    public SettingsPageViewModel(IConfigService configService, IStatusService status)
    {
        _configService = configService;
        _status = status;

        _lastQuickWriteDir = _configService.AppConfig.LastQuickWriteDir ?? string.Empty;
        _providers = new ObservableCollection<AiProviderConfig>(_configService.AppConfig.AiProviders);
    }

    // ------------------------------------------------------------------
    // App tab
    // ------------------------------------------------------------------

    /// <summary>Last directory used by the Quick Write feature (persisted string).</summary>
    [ObservableProperty]
    private string _lastQuickWriteDir = string.Empty;

    /// <summary>Persists the App tab fields.</summary>
    [RelayCommand]
    private void SaveQuickWriteDir()
    {
        SaveConfig();
        _status.Enqueue("Quick write directory saved.");
    }

    // ------------------------------------------------------------------
    // AI tab — sidebar
    // ------------------------------------------------------------------

    /// <summary>The provider currently selected in the sidebar, or <c>null</c>.</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasProviderSelection))]
    [NotifyPropertyChangedFor(nameof(ProviderFormTitle))]
    [NotifyPropertyChangedFor(nameof(CanDeleteProvider))]
    private AiProviderConfig? _selectedProvider;

    /// <summary>All configured AI providers; shown in the sidebar.</summary>
    public ObservableCollection<AiProviderConfig> Providers => _providers;

    /// <summary>Number of configured providers, used by the sidebar header badge.</summary>
    public int ProviderCount => _providers.Count;

    /// <summary>True when at least one provider is configured.</summary>
    public bool HasAnyProviders => _providers.Count > 0;

    /// <summary>
    /// True while the content pane shows the provider form (either an existing
    /// provider is selected or a new one is being composed).
    /// </summary>
    public bool HasProviderSelection => SelectedProvider is not null || IsComposingNewProvider;

    /// <summary>True when the selected sidebar entry can be deleted (only existing providers).</summary>
    public bool CanDeleteProvider => SelectedProvider is not null;

    // ------------------------------------------------------------------
    // AI tab — form (mutable working copy of the selected / new provider)
    // ------------------------------------------------------------------

    /// <summary>True while an empty form for a brand-new provider is open.</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasProviderSelection))]
    [NotifyPropertyChangedFor(nameof(IsEditingNewProvider))]
    [NotifyPropertyChangedFor(nameof(ProviderFormTitle))]
    [NotifyPropertyChangedFor(nameof(SaveProviderButtonText))]
    private bool _isComposingNewProvider;

    /// <summary>Provider display name, e.g. "OpenRouter".</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanSaveProvider))]
    private string _providerName = string.Empty;

    /// <summary>
    /// Provider kind as a string ("None" or an <see cref="AiProvider"/> enum name).
    /// Kept as text so the ComboBox can offer a nullable "None" option without
    /// tripping over null items under Avalonia's compiled bindings.
    /// </summary>
    [ObservableProperty]
    private string _providerType = "None";

    /// <summary>Base URL of the provider's API, e.g. "https://openrouter.ai/api/v1".</summary>
    [ObservableProperty]
    private string _providerBaseUrl = string.Empty;

    /// <summary>API key for the provider (masked in the UI).</summary>
    [ObservableProperty]
    private string _providerApiKey = string.Empty;

    /// <summary>Combo box choices: "None" followed by every <see cref="AiProvider"/> enum name.</summary>
    public IReadOnlyList<string> ProviderTypeOptions { get; } =
        new[] { "None" }.Concat(Enum.GetNames<AiProvider>()).ToArray();

    /// <summary>Form heading: provider name when editing, "New Provider" when composing.</summary>
    public string ProviderFormTitle =>
        IsComposingNewProvider ? "New Provider" : SelectedProvider?.Name ?? "Provider";

    /// <summary>Save button label depends on whether this is a new or existing provider.</summary>
    public string SaveProviderButtonText => IsComposingNewProvider ? "Add Provider" : "Save Provider";

    /// <summary>True while composing a brand-new provider (drives the "Add" button label).</summary>
    public bool IsEditingNewProvider => IsComposingNewProvider;

    /// <summary>A provider needs at least a name before it can be saved.</summary>
    public bool CanSaveProvider => !string.IsNullOrWhiteSpace(ProviderName);

    /// <summary>Populates the form when an existing provider is picked from the sidebar.</summary>
    partial void OnSelectedProviderChanged(AiProviderConfig? value)
    {
        if (value is null)
        {
            return;
        }

        IsComposingNewProvider = false;
        LoadForm(value);
    }

    /// <summary>Clears the form and opens it for a brand-new provider.</summary>
    [RelayCommand]
    private void NewProvider()
    {
        SelectedProvider = null;
        IsComposingNewProvider = true;
        ClearForm();
    }

    /// <summary>
    /// Builds a provider record from the form. A new provider is appended to the
    /// sidebar list; an existing one replaces its entry in place.
    /// </summary>
    [RelayCommand]
    private void SaveProvider()
    {
        var config = new AiProviderConfig(
            ProviderName.Trim(),
            ProviderType == "None" ? null : Enum.Parse<AiProvider>(ProviderType),
            ProviderBaseUrl.Trim(),
            ProviderApiKey.Trim());

        if (IsComposingNewProvider)
        {
            _providers.Add(config);
            IsComposingNewProvider = false;
            SelectedProvider = config;
            _status.Enqueue("AI provider added.");
        }
        else if (SelectedProvider is { } existing)
        {
            var index = _providers.IndexOf(existing);
            if (index >= 0)
            {
                _providers[index] = config;
            }

            SelectedProvider = config;
            _status.Enqueue("AI provider saved.");
        }

        OnPropertyChanged(nameof(ProviderCount));
        OnPropertyChanged(nameof(HasAnyProviders));
        SaveConfig();
    }

    /// <summary>Removes the selected provider from the configuration.</summary>
    [RelayCommand]
    private void DeleteProvider()
    {
        if (SelectedProvider is not { } provider)
        {
            return;
        }

        _providers.RemoveAt(_providers.IndexOf(provider));
        SelectedProvider = null;
        IsComposingNewProvider = false;
        ClearForm();

        OnPropertyChanged(nameof(ProviderCount));
        OnPropertyChanged(nameof(HasAnyProviders));
        SaveConfig();
        _status.Enqueue("AI provider deleted.");
    }

    // ------------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------------

    /// <summary>Copies an existing provider record into the editable form fields.</summary>
    private void LoadForm(AiProviderConfig config)
    {
        ProviderName = config.Name;
        ProviderType = config.Provider?.ToString() ?? "None";
        ProviderBaseUrl = config.BaseUrl;
        ProviderApiKey = config.ApiKey;
    }

    /// <summary>Resets the form fields to their empty defaults.</summary>
    private void ClearForm()
    {
        ProviderName = string.Empty;
        ProviderType = "None";
        ProviderBaseUrl = string.Empty;
        ProviderApiKey = string.Empty;
    }

    /// <summary>
    /// Writes the current page state back to <see cref="IConfigService"/> and persists
    /// it to the config file.
    /// </summary>
    private void SaveConfig()
    {
        var directory = string.IsNullOrWhiteSpace(LastQuickWriteDir) ? null : LastQuickWriteDir.Trim();
        _configService.AppConfig = new AppConfig(directory, _providers.ToList());
        _configService.StoreConfig();
    }
}
