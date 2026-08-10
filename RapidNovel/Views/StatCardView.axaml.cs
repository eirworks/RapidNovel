using Avalonia;
using Avalonia.Controls;

namespace RapidNovel.Views;

/// <summary>
/// A reusable stat card displaying a <see cref="Title"/> and a <see cref="Value"/>.
/// No ViewModel required — set the properties directly from XAML:
/// <code>&lt;controls:StatCard Title="Orders" Value="123" /&gt;</code>
/// </summary>
public partial class StatCard : UserControl
{
    /// <summary>Identifies the <see cref="Title"/> dependency property.</summary>
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<StatCard, string>(nameof(Title), defaultValue: string.Empty);

    /// <summary>Identifies the <see cref="Value"/> dependency property.</summary>
    public static readonly StyledProperty<string> ValueProperty =
        AvaloniaProperty.Register<StatCard, string>(nameof(Value), defaultValue: string.Empty);

    public StatCard()
    {
        InitializeComponent();
    }

    /// <summary>Gets or sets the card title, e.g. "Orders".</summary>
    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>Gets or sets the card value, e.g. "123".</summary>
    public string Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
}
