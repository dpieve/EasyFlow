using Avalonia.Collections;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyFlow.Application.Settings;
using EasyFlow.Domain.Entities;
using EasyFlow.Presentation.Common;
using EasyFlow.Presentation.Services;
using MediatR;
using ReactiveUI;
using SukiUI;
using SukiUI.Controls;
using SukiUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace EasyFlow.Presentation;

public partial class MainViewModel : ViewModelBase
{
    private readonly SukiTheme _theme;
    private readonly IMediator _mediator;
    private readonly IRestartAppService _restartAppService;
    private readonly ILanguageService _languageService;

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
        IMediator mediator,
        IRestartAppService restartAppService,
        ILanguageService languageService)
    {
        Pages = new AvaloniaList<PageViewModelBase>(pages.OrderBy(x => x.Index));
        _mediator = mediator;
        _restartAppService = restartAppService;
        _languageService = languageService;

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
            SukiHost.ShowToast(_languageService.GetString("Success"), $"{_languageService.GetString("ChangedThemeTo")} {variant}", SukiUI.Enums.NotificationType.Success);
        };

        _theme.OnColorThemeChanged += theme =>
        {
            SukiHost.ShowToast(_languageService.GetString("Success"), $"{_languageService.GetString("ChangedColorTo")} {theme.DisplayName}.", SukiUI.Enums.NotificationType.Success);
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
            return;
        }

        if (prevLanguage != SelectedLanguage.Code)
        {
            _restartAppService.Restart();
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
}