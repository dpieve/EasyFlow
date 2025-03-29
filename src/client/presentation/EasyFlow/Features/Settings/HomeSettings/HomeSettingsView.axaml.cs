using Avalonia.Controls;
using System;

namespace EasyFlow.Features.Settings.HomeSettings;

public partial class HomeSettingsView : UserControl
{
    public HomeSettingsView()
    {
        InitializeComponent();

        if (OperatingSystem.IsLinux())
        {
            PowerOffButton.IsVisible = true;
        }
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {

        Environment.Exit(0);
    }
}