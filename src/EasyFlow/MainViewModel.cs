using Avalonia.Collections;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyFlow.Common;
using SukiUI;
using SukiUI.Controls;
using SukiUI.Models;
using System.Collections.Generic;
using System.Linq;

namespace EasyFlow;

public partial class MainViewModel : ViewModelBase
{
    private readonly SukiTheme _theme;

    [ObservableProperty]
    private ThemeVariant _baseTheme;

    [ObservableProperty]
    private PageViewModelBase? _activePage;

    public MainViewModel(IEnumerable<PageViewModelBase> pages)
    {
        Pages = new AvaloniaList<PageViewModelBase>(pages.OrderBy(x => x.Index));
        _theme = SukiTheme.GetInstance();

        Themes = _theme.ColorThemes;
        BaseTheme = _theme.ActiveBaseTheme;

        _theme.OnBaseThemeChanged += variant =>
        {
            BaseTheme = variant;
            SukiHost.ShowToast("Successfully Changed Theme", $"Changed Theme To {variant}", SukiUI.Enums.NotificationType.Success);
        };

        _theme.OnColorThemeChanged += theme =>
            SukiHost.ShowToast("Successfully Changed Color", $"Changed Color To {theme.DisplayName}.", SukiUI.Enums.NotificationType.Success);
    }

    partial void OnActivePageChanged(PageViewModelBase? oldValue, PageViewModelBase? newValue)
    {
        if (oldValue is not null)
        {
            oldValue.IsActive = false;
        }

        if (newValue is not null)
        {
            newValue.IsActive = true;
        }
    }

    public IAvaloniaReadOnlyList<PageViewModelBase> Pages { get; }

    public IAvaloniaReadOnlyList<SukiColorTheme> Themes { get; }

    [RelayCommand]
    private void ToggleBaseTheme() =>
        _theme.SwitchBaseTheme();

    public void ChangeTheme(SukiColorTheme theme) =>
        _theme.ChangeColorTheme(theme);
}