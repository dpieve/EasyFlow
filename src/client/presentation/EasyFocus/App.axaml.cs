using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using EasyFocus.Common;
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
using Serilog;
using Splat;
using System;
using System.Threading.Tasks;

namespace EasyFocus;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        var mainViewModel = await Bootstrap.CreateMainViewModel();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = mainViewModel
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var exception = e.ExceptionObject as Exception;
        Log.Fatal(exception, "Unhandled Exception: {Message}", exception?.Message);
    }
}

public static class  Bootstrap
{
    public static async Task<MainViewModel> CreateMainViewModel()
    {
        var settingsService = Locator.Current.GetServiceOrThrow<ISettingsService>();
        await settingsService.Initialize(); // Seed Data

        var playSoundService = Locator.Current.GetServiceOrThrow<IPlaySoundService>();
        var notificationService = Locator.Current.GetServiceOrThrow<INotificationService>();
        var sessionService = Locator.Current.GetServiceOrThrow<ISessionService>();
        var tagsService = Locator.Current.GetServiceOrThrow<ITagService>();
        var browserService = Locator.Current.GetServiceOrThrow<IBrowserService>();
        var appSettings = await settingsService.GetSettingsAsync(1) ?? throw new InvalidOperationException("Settings can't be null");

        var homeVm = new HomeSettingsViewModel(browserService);
        var focusVm = new FocusTimeViewModel(appSettings, settingsService);
        var notificationsVm = new NotificationsViewModel(appSettings, settingsService);
        var tagsVm = new TagsViewModel(tagsService);
        var backgroundVm = new BackgroundViewModel(appSettings, settingsService);
        var settingsVm = new SettingsViewModel(homeVm, focusVm, notificationsVm, tagsVm, backgroundVm);
        var pomodoroVm = new PomodoroViewModel(settingsVm, playSoundService, notificationService, sessionService, browserService);
        var reportVm = new ReportViewModel(sessionService);

        return new MainViewModel(
            settingsVm,
            pomodoroVm,
            reportVm);
    }
}