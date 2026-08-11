using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using RapidNovel.Models.Enums;

namespace RapidNovel.Converters;

/// <summary>
/// Maps a <see cref="StatusSeverity"/> to the dot color used in the status bar.
/// </summary>
public class StatusSeverityToBrushConverter : IValueConverter
{
    public static readonly StatusSeverityToBrushConverter Instance = new();

    private static readonly ISolidColorBrush InfoBrush = new SolidColorBrush(Color.Parse("#2B6CB0"));
    private static readonly ISolidColorBrush SuccessBrush = new SolidColorBrush(Color.Parse("#2F855A"));
    private static readonly ISolidColorBrush WarningBrush = new SolidColorBrush(Color.Parse("#B7791F"));
    private static readonly ISolidColorBrush ErrorBrush = new SolidColorBrush(Color.Parse("#C53030"));

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            StatusSeverity.Success => SuccessBrush,
            StatusSeverity.Warning => WarningBrush,
            StatusSeverity.Error => ErrorBrush,
            _ => InfoBrush,
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
