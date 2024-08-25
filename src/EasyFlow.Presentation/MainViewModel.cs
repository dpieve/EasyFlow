using Avalonia.Collections;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyFlow.Presentation.Common;
using EasyFlow.Presentation.Services;
using SukiUI;
using SukiUI.Controls;
using SukiUI.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EasyFlow.Presentation;

public partial class MainViewModel : ViewModelBase
{
    private readonly SukiTheme _theme;
    private readonly IGeneralSettingsService _generalSettingsService;

    [ObservableProperty]
    private ThemeVariant _baseTheme;

    [ObservableProperty]
    private PageViewModelBase? _activePage;

    public MainViewModel(IEnumerable<PageViewModelBase> pages, IGeneralSettingsService generalSettingsService)
    {
        Pages = new AvaloniaList<PageViewModelBase>(pages.OrderBy(x => x.Index));
        _generalSettingsService = generalSettingsService;

        _theme = SukiTheme.GetInstance();
        Themes = _theme.ColorThemes;

        var savedTheme = LoadTheme();
        if (savedTheme.ToString() != _theme.ActiveBaseTheme.ToString())
        {
            ToggleBaseTheme();
        }

        BaseTheme = _theme.ActiveBaseTheme;

        var colorTheme = LoadColorTheme();
        ChangeTheme(colorTheme);

        _theme.OnBaseThemeChanged += variant =>
        {
            _generalSettingsService.UpdateSelectedTheme(variant.ToTheme());

            BaseTheme = variant;
            SukiHost.ShowToast("Successfully Changed Theme", $"Changed Theme To {variant}", SukiUI.Enums.NotificationType.Success);
        };

        _theme.OnColorThemeChanged += theme =>
        {
            _generalSettingsService.UpdateSelectedColorTheme(theme.ToColorTheme());
            SukiHost.ShowToast("Successfully Changed Color", $"Changed Color To {theme.DisplayName}.", SukiUI.Enums.NotificationType.Success);
        };
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
    public void ChangeTheme(SukiColorTheme theme)
    {
        _theme.ChangeColorTheme(theme);
    }

    [RelayCommand]
    private void ToggleBaseTheme()
    {
        _theme.SwitchBaseTheme();
    }

    [RelayCommand]
    private async Task ChangeLanguage(SupportedLanguage selectedLanguage)
    {
        var resultSelectedLanguage = _generalSettingsService.GetSelectedLanguage();
        if (resultSelectedLanguage.Error is not null)
        {
            await SukiHost.ShowToast("Failed to update the language", $"Selected language {selectedLanguage.Name}", SukiUI.Enums.NotificationType.Error);
            return;
        }
        
        var language = resultSelectedLanguage.Value!;
        if (language.Code == selectedLanguage.Code)
        {
            await SukiHost.ShowToast("Language already selected", "No changes made", SukiUI.Enums.NotificationType.Info);
            return;
        }

        var result = await _generalSettingsService.UpdateSelectedLanguage(selectedLanguage);
        if (result.Error is not null)
        {
            await SukiHost.ShowToast("Failed to update the language", $"Selected language {selectedLanguage.Name}", SukiUI.Enums.NotificationType.Error);
            return;
        }
        await SukiHost.ShowToast("Changed language", $"Selected language {selectedLanguage.Name}", SukiUI.Enums.NotificationType.Success);
        
        string exePath = Process.GetCurrentProcess().MainModule.FileName;
        Process.Start(exePath);
        Process.GetCurrentProcess().Kill();
    }

    private ThemeVariant LoadTheme()
    {
        var result = _generalSettingsService.Get();
        if (result.Error is not null)
        {
            return ThemeVariant.Dark;
        }
        var settings = result.Value!;
        return settings.SelectedTheme.ToThemeVariant();
    }

    private SukiColorTheme LoadColorTheme()
    {
       var result = _generalSettingsService.Get();
        if (result.Error is not null)
        {
            return _theme.ColorThemes.First(theme => theme.DisplayName == "Red");
        }
        var settings = result.Value!;
        var selectedTheme = settings.SelectedColorTheme;

        var colorTheme = _theme.ColorThemes.First(theme => theme.DisplayName == selectedTheme.ToString());
        return colorTheme;
    }
}