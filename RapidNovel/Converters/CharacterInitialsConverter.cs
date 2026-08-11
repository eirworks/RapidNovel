using System;
using System.Globalization;
using Avalonia.Data.Converters;
using RapidNovel.Models;

namespace RapidNovel.Converters;

/// <summary>
/// Turns a <see cref="Character"/> into its avatar initials (first letter of the
/// first name, upper-cased). Used by the wiki header avatar circle. Falls back to
/// "?" for characters without a first name.
/// </summary>
public class CharacterInitialsConverter : IValueConverter
{
    public static readonly CharacterInitialsConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Character character || string.IsNullOrWhiteSpace(character.FirstName))
        {
            return "?";
        }

        return character.FirstName[..1].ToUpperInvariant();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
