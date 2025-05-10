using EasyFocus.Domain.Entities;
using EasyFocus.Domain.Services;
using EasyFocus.Features;
using EasyFocus.Features.Pomodoro;
using EasyFocus.Features.Report;
using EasyFocus.Features.Settings;
using EasyFocus.Features.Settings.Background;
using EasyFocus.Features.Settings.FocusTime;
using EasyFocus.Features.Settings.HomeSettings;
using EasyFocus.Features.Settings.Notifications;
using EasyFocus.Features.Settings.Tags;
using Splat;

namespace EasyFocus.Resources.Mockups;

/// <summary>
/// This class exists just for the Live Preview.
/// </summary>
public static class LivePreviewMockup
{
    // Services.
    public static IBrowserService BrowserService { get; } = new BrowserServiceMockup();

    public static IAppSettingsService AppSettingsService { get; } = new AppSettingsServiceMockup();
    public static ITagService TagService { get; } = new TagServiceMockup();
    public static IAudioService AudioService { get; } = new AudioServiceMockup();
    public static INotificationService NotificationService { get; } = new NotificationServiceMockup();
    public static ISessionService SessionService { get; } = new SessionServiceMockup();

    // Settings.
    public static AppSettings AppSettings { get; } = AppSettings.CreateDefault();

    // ViewModels.
    public static HomeSettingsViewModel HomeSettingsViewModel { get; } = new HomeSettingsViewModel(BrowserService);

    public static FocusTimeViewModel FocusTimeViewModel { get; } = new FocusTimeViewModel(AppSettings, AppSettingsService);
    public static NotificationsViewModel NotificationsViewModel { get; } = new NotificationsViewModel(AppSettings, AppSettingsService);
    public static TagsViewModel TagsViewModel { get; } = new TagsViewModel(TagService);
    public static BackgroundViewModel BackgroundViewModel { get; } = new BackgroundViewModel(AppSettings, AppSettingsService);

    public static SettingsViewModel SettingsViewModel { get; } = new SettingsViewModel(HomeSettingsViewModel,
     FocusTimeViewModel,
     NotificationsViewModel,
     TagsViewModel,
     BackgroundViewModel);

    public static PomodoroViewModel PomodoroViewModel { get; } = new PomodoroViewModel(SettingsViewModel,
        AudioService,
        NotificationService,
        SessionService,
        BrowserService);

    public static ReportViewModel ReportViewModel { get; } = new ReportViewModel(SessionService);

    public static MainViewModel MainViewModel { get; } = new MainViewModel(SettingsViewModel,
        PomodoroViewModel,
        ReportViewModel);

    public static void Register()
    {
        Locator.CurrentMutable.RegisterConstant(HomeSettingsViewModel);
        Locator.CurrentMutable.RegisterConstant(FocusTimeViewModel);
        Locator.CurrentMutable.RegisterConstant(NotificationsViewModel);
        Locator.CurrentMutable.RegisterConstant(TagsViewModel);
        Locator.CurrentMutable.RegisterConstant(BackgroundViewModel);
        Locator.CurrentMutable.RegisterConstant(SettingsViewModel);
        Locator.CurrentMutable.RegisterConstant(PomodoroViewModel);
        Locator.CurrentMutable.RegisterConstant(ReportViewModel);
        Locator.CurrentMutable.RegisterConstant(MainViewModel);
        Locator.CurrentMutable.RegisterConstant(AppSettings);

        Locator.CurrentMutable.RegisterConstant(BrowserService);
        Locator.CurrentMutable.RegisterConstant(AppSettingsService);
        Locator.CurrentMutable.RegisterConstant(TagService);
        Locator.CurrentMutable.RegisterConstant(AudioService);
        Locator.CurrentMutable.RegisterConstant(NotificationService);
        Locator.CurrentMutable.RegisterConstant(SessionService);
    }
}
