using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;

namespace RapidNovel.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void ExitMenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void AboutMenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        new AboutWindow().ShowDialog(this);
    }
}