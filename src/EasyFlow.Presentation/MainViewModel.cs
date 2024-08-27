using Avalonia.Collections;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyFlow.Application.Settings;
using EasyFlow.Domain.Entities;
using EasyFlow.Presentation.Common;
using ReactiveUI;
using SukiUI;
using SukiUI.Controls;
using SukiUI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System;
using System.Diagnostics;
using MediatR;

namespace EasyFlow.Presentation;

public partial class MainViewModel : ViewModelBase
{
    private readonly SukiTheme _theme;
    private readonly IMediator _mediator;

    [ObservableProperty]
    private ThemeVariant _baseTheme;

    [ObservableProperty]
    private SukiColorTheme _selectedTheme;

    [ObservableProperty]
    private PageViewModelBase? _activePage;

    [ObservableProperty]
    private SupportedLanguage _selectedLanguage;

    public MainViewModel(
        IEnumerable<PageViewModelBase> pages,
        IMediator mediator)
    {
        Pages = new AvaloniaList<PageViewModelBase>(pages.OrderBy(x => x.Index));
        _mediator = mediator;
        _theme = SukiTheme.GetInstance();
        Themes = _theme.ColorThemes;

        var settings = GetSettings().GetAwaiter().GetResult();
        SelectedLanguage = SupportedLanguage.FromCode(settings.SelectedLanguage);

        var colorTheme = _theme.ColorThemes.First(theme => theme.DisplayName == settings.SelectedColorTheme.ToString());
        SelectedTheme = colorTheme;

        var savedTheme = settings.SelectedTheme.ToThemeVariant();
        if (savedTheme.ToString() != _theme.ActiveBaseTheme.ToString())
        {
            ToggleBaseTheme();
        }
        
        BaseTheme = _theme.ActiveBaseTheme;
        
        _theme.OnBaseThemeChanged += variant =>
        {
            BaseTheme = variant;
            SukiHost.ShowToast("Successfully Changed Theme", $"Changed Theme To {variant}", SukiUI.Enums.NotificationType.Success);
        };

        _theme.OnColorThemeChanged += theme =>
        {
            SukiHost.ShowToast("Successfully Changed Color", $"Changed Color To {theme.DisplayName}.", SukiUI.Enums.NotificationType.Success);
        };

        this.WhenAnyValue(
                vm => vm.SelectedTheme, 
                vm => vm.BaseTheme, 
                vm => vm.SelectedLanguage)
            .Skip(1)
            .Select(_ => System.Reactive.Unit.Default)
            .ObserveOn(RxApp.MainThreadScheduler)
            .InvokeCommand(UpdateSettingsCommand);

        this.WhenAnyValue(vm => vm.SelectedTheme)
            .Subscribe(ChangeTheme);
    }

    private async Task<GeneralSettings> GetSettings()
    {
        var result = await _mediator.Send(new GetSettingsQuery());

        if (result.IsSuccess)
        {
            return result.Value;
        }

        await SukiHost.ShowToast("Failed to load", "Settings couldn't be loaded.", SukiUI.Enums.NotificationType.Error);
        return new GeneralSettings();
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
    private async Task UpdateSettings()
    {
        var settings = await GetSettings();

        settings.SelectedTheme = BaseTheme.ToTheme();
        settings.SelectedColorTheme = SelectedTheme.ToColorTheme();

        var prevLanguage = settings.SelectedLanguage;

        settings.SelectedLanguage = SelectedLanguage.Code;

        var command = new UpdateSettingsCommand
        {
            GeneralSettings = settings
        };

        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
        {
            await SukiHost.ShowToast("Failed to update", "Failed to update the settings", SukiUI.Enums.NotificationType.Error);
        }

        if (prevLanguage != SelectedLanguage.Code)
        {
            RestartApp();
        }
    }


    [RelayCommand]
    private void ToggleBaseTheme()
    {
        _theme.SwitchBaseTheme();
    }

    [RelayCommand]
    private void ChangeLanguage(SupportedLanguage selectedLanguage)
    {
        SelectedLanguage = selectedLanguage;
    }

    private static void RestartApp()
    {
        try
        {
            var mainModule = Process.GetCurrentProcess().MainModule;
            if (mainModule is null)
            {
                return;
            }

            string exePath = mainModule.FileName;
            Process.Start(exePath);

            Process.GetCurrentProcess().Kill();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }
}