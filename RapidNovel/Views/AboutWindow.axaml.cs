using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using RapidNovel.Models.Enums;
using RapidNovel.Services.Status;

namespace RapidNovel.Views;

public partial class AboutWindow : Window
{
    public AboutWindow()
    {
        InitializeComponent();
        VersionText.Text =
            $"Version {Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "1.0.0"}";

        // Demo: windows created with `new` can reach the status bar through App.Services.
        Opened += (_, _) =>
            App.Services?.GetRequiredService<IStatusService>()
                .Enqueue("Opened About window", StatusSeverity.Info);
    }

    private void CloseButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
