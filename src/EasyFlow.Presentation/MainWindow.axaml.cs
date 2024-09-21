using Avalonia.Controls;
using Avalonia.Interactivity;
using SukiUI.Controls;
using SukiUI.Models;

namespace EasyFlow.Presentation;

public partial class MainWindow : SukiWindow
{
    public MainWindow()
    {
        InitializeComponent();

        //SukiHost.SetToastLimit(this, 3);
    }

    private void MenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel vm)
        {
            return;
        }

        if (e.Source is not MenuItem menuItem)
        {
            return;
        }

        if (menuItem.DataContext is not SukiColorTheme colorTheme)
        {
            return;
        }

        vm.SelectedTheme = colorTheme;
    }
}