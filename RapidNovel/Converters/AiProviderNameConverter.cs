using System;
using System.Globalization;
using Avalonia.Data.Converters;
using RapidNovel.Models.Enums;

namespace RapidNovel.Converters;

/// <summary>
/// Renders an <see cref="AiProvider"/> value as its display name (e.g. "OpenRouter"),
/// mapping <c>null</c> to "None". Used by the AI providers sidebar to label the
/// provider type of each entry.
/// </summary>
public class AiProviderNameConverter : IValueConverter
{
    public static readonly AiProviderNameConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is AiProvider provider ? provider.ToString() : "None";

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
