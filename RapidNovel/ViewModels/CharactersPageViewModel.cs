using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RapidNovel.Models;
using RapidNovel.Models.Interfaces;

namespace RapidNovel.ViewModels;

/// <summary>
/// View model for the Characters page: a searchable sidebar list of the open
/// project's characters plus a wiki-style detail pane for the selected one.
/// The page is read-only for now — characters come straight from
/// <see cref="IProjectService.Project"/>.Database.Characters.
/// </summary>
public partial class CharactersPageViewModel : ViewModelBase
{
    private readonly IProjectService _projectService;

    /// <summary>All characters of the currently open project (unfiltered).</summary>
    private IReadOnlyList<Character> _allCharacters = Array.Empty<Character>();

    /// <summary>Design-time / previewer support: seeds a sample project.</summary>
    public CharactersPageViewModel()
        : this(new ProjectService { Project = SampleProject() })
    {
    }

    public CharactersPageViewModel(IProjectService projectService)
    {
        _projectService = projectService;
        RefreshCharacters();

        // Keep the page in sync when a project is created/loaded elsewhere.
        _projectService.ProjectChanged += (_, _) => RefreshCharacters();
    }

    /// <summary>Text used to filter the sidebar list (matched against names and aliases).</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredCharacters))]
    [NotifyPropertyChangedFor(nameof(HasFilteredCharacters))]
    [NotifyPropertyChangedFor(nameof(CharacterCount))]
    private string _searchText = string.Empty;

    /// <summary>The character shown in the wiki-style content pane.</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelection))]
    [NotifyPropertyChangedFor(nameof(HasAliases))]
    [NotifyPropertyChangedFor(nameof(HasDescription))]
    [NotifyPropertyChangedFor(nameof(SelectedBirthdayText))]
    private Character? _selectedCharacter;

    /// <summary>True while a character is selected; drives which content pane shows.</summary>
    public bool HasSelection => SelectedCharacter is not null;

    /// <summary>True when the open project contains at least one character.</summary>
    public bool HasAnyCharacters => _allCharacters.Count > 0;

    /// <summary>True when the filtered sidebar list is non-empty.</summary>
    public bool HasFilteredCharacters => FilteredCharacters.Count > 0;

    /// <summary>Number of characters currently shown, used by the sidebar header badge.</summary>
    public int CharacterCount => FilteredCharacters.Count;

    /// <summary>True when the selected character has at least one alias.</summary>
    public bool HasAliases => SelectedCharacter?.Aliases is { Count: > 0 };

    /// <summary>True when the selected character has a non-blank description.</summary>
    public bool HasDescription => !string.IsNullOrWhiteSpace(SelectedCharacter?.Description);

    /// <summary>
    /// Formatted birthday of the selected character (e.g. "June 4, 1994"),
    /// or empty when unset. Formatted here rather than with a XAML
    /// <c>StringFormat</c> because commas inside a format string break
    /// Avalonia's markup-extension parser.
    /// </summary>
    public string SelectedBirthdayText =>
        SelectedCharacter?.Birthday is { } birthday ? birthday.ToString("MMMM d, yyyy") : string.Empty;

    /// <summary>
    /// The sidebar list: all characters, or those matching <see cref="SearchText"/>
    /// (case-insensitive contains over first name, last name and aliases).
    /// </summary>
    public IReadOnlyList<Character> FilteredCharacters =>
        string.IsNullOrWhiteSpace(SearchText)
            ? _allCharacters
            : _allCharacters.Where(MatchesSearch).ToList();

    /// <summary>
    /// Re-reads the character list from the open project and resets the page state.
    /// </summary>
    private void RefreshCharacters()
    {
        _allCharacters = _projectService.Project?.Database.Characters ?? new List<Character>();
        SearchText = string.Empty;
        SelectedCharacter = null;

        OnPropertyChanged(nameof(FilteredCharacters));
        OnPropertyChanged(nameof(HasAnyCharacters));
        OnPropertyChanged(nameof(HasFilteredCharacters));
        OnPropertyChanged(nameof(CharacterCount));
    }

    /// <summary>True when the character matches the current search text.</summary>
    private bool MatchesSearch(Character character)
    {
        var needle = SearchText.Trim();
        if (needle.Length == 0) return true;

        var haystack = string.Join(
            ' ',
            new[] { character.FirstName, character.LastName }
                .Concat(character.Aliases ?? Enumerable.Empty<string>()));

        return haystack.Contains(needle, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Creates a new character. Currently a UI stub — the creation flow
    /// (dialog or inline editor) is not implemented yet.
    /// </summary>
    [RelayCommand]
    private void NewCharacter()
    {
        // TODO: character creation flow is not implemented yet (UI stub).
    }

    /// <summary>Builds a sample project used only by the IDE previewer.</summary>
    private static Project SampleProject()
    {
        return new Project
        {
            Id = "design-time",
            Name = "Sample Novel",
            Author = "Design",
            Database = new ProjectDb
            {
                Characters = new List<Character>
                {
                    new(
                        "c1",
                        "Aria",
                        "Voss",
                        true,
                        new DateTime(1994, 6, 4),
                        "A stubborn cartographer who maps the spaces between worlds and "
                        + "collects the stories strangers leave behind in railway stations.",
                        new List<string> { "Ari", "The Cartographer" }),
                    new(
                        "c2",
                        "Jonas",
                        "Reed",
                        false,
                        null,
                        null,
                        new List<string> { "J.R." }),
                },
            },
        };
    }
}
