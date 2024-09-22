using Avalonia.Controls;
using Avalonia.Interactivity;
using SukiUI.Controls;
using SukiUI.Models;

namespace EasyFlow.Desktop;

public partial class MainWindow : SukiWindow
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public bool ShouldClose { get; set; }
    public void ToTray()
    {
        ShowInTaskbar = false;
        Hide();
    }

    public void FromTray()
    {
        ShowInTaskbar = true;
        this.BringIntoView();
        Activate();
        Focus();
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        if (!ShouldClose)
        {
            e.Cancel = true;
            ToTray();
        }
        else
        {
            e.Cancel = false;
        }
    }

    private void SelectTheme_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel vm ||
            e.Source is not MenuItem menuItem ||
            menuItem.DataContext is not SukiColorTheme colorTheme)
        {
            return;
        }

        vm.SelectedTheme = colorTheme;
    }
}