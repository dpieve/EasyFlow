using Avalonia.Collections;
using Avalonia.Controls.Notifications;
using Avalonia.Styling;
using EasyFlow.Desktop.Common;
using EasyFlow.Desktop.Services;
using EasyFlow.Domain.Entities;
using MediatR;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using SukiUI;
using SukiUI.Dialogs;
using SukiUI.Models;
using System.Reactive.Linq;

namespace EasyFlow.Desktop;

public partial class MainViewModel : ViewModelBase
{
    private readonly SukiTheme _theme;
    private readonly IMediator _mediator;
    private readonly IRestartAppService _restartAppService;
    private readonly ILanguageService _languageService;
    private readonly INotificationService _notificationService;

    private bool showStillRunning = true;

    [Reactive]
    private ThemeVariant _baseTheme;

    [Reactive]
    private SukiColorTheme _selectedTheme;

    [Reactive]
    private SideMenuViewModelBase? _activePage;

    [Reactive]
    private SupportedLanguage _selectedLanguage;

    public MainViewModel(
        IEnumerable<SideMenuViewModelBase> pages,
        IMediator mediator,
        IRestartAppService restartAppService,
        ILanguageService languageService,
        IToastService toastService,
        ISukiDialogManager dialogManager,
        INotificationService notificationService)
    {
        Pages = new AvaloniaList<SideMenuViewModelBase>(pages.OrderBy(x => x.Index));
        _mediator = mediator;
        _restartAppService = restartAppService;
        _languageService = languageService;
        ToastService = toastService;
        DialogManager = dialogManager;
        _notificationService = notificationService;
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
            var themeName = _languageService.GetString(variant.ToString());
            ToastService.Display(_languageService.GetString("Success"), $"{_languageService.GetString("ChangedThemeTo")} {themeName}", NotificationType.Success);
        };

        _theme.OnColorThemeChanged += theme =>
        {
            var themeName = _languageService.GetString(theme.DisplayName);
            ToastService.Display(_languageService.GetString("Success"), $"{_languageService.GetString("ChangedColorTo")} {themeName}.", NotificationType.Success);
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

    public IAvaloniaReadOnlyList<SideMenuViewModelBase> Pages { get; }
    public IAvaloniaReadOnlyList<SukiColorTheme> Themes { get; }
    public IToastService ToastService { get; }
    public ISukiDialogManager DialogManager { get; }

    public void ChangeTheme(SukiColorTheme theme)
    {
        _theme.ChangeColorTheme(theme);
    }

    public void ShowStillRunningNotification()
    {
        if (showStillRunning)
        {
            _notificationService.Show(ConstantTranslation.AppStillRunning, ConstantTranslation.FindAppInSystemTray);
            showStillRunning = false;
        }
    }

    [ReactiveCommand]
    private async Task UpdateSettings()
    {
        var settings = await GetSettings();
        settings.SelectedTheme = BaseTheme.ToTheme();
        settings.SelectedColorTheme = SelectedTheme.ToColorTheme();
        var prevLanguage = settings.SelectedLanguage;
        settings.SelectedLanguage = SelectedLanguage.Code;

        var result = await _mediator.Send(new Application.Settings.Edit.Command { Settings = settings });
        if (!result.IsSuccess)
        {
            return;
        }

        if (prevLanguage != SelectedLanguage.Code)
        {
            _restartAppService.Restart();
        }
    }

    [ReactiveCommand]
    private void ToggleBaseTheme()
    {
        _theme.SwitchBaseTheme();
    }

    [ReactiveCommand]
    private void ChangeLanguage(SupportedLanguage selectedLanguage)
    {
        SelectedLanguage = selectedLanguage;
    }

    private async Task<GeneralSettings> GetSettings()
    {
        var result = await _mediator.Send(new Application.Settings.Get.Query());

        if (result.IsSuccess)
        {
            return result.Value;
        }

        return new GeneralSettings();
    }
}