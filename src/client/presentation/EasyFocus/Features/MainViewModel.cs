using EasyFocus.Common;
using EasyFocus.Domain.Services;
using EasyFocus.Features.Pomodoro;
using EasyFocus.Features.Report;
using EasyFocus.Features.Settings;
using EasyFocus.Features.Settings.Background;
using EasyFocus.Features.Settings.FocusTime;
using EasyFocus.Features.Settings.HomeSettings;
using EasyFocus.Features.Settings.Notifications;
using EasyFocus.Features.Settings.Tags;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Serilog;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace EasyFocus.Features;

public sealed partial class MainViewModel : ViewModelBase
{
    [Reactive] private ViewModelBase _currentViewModel;
    [Reactive] private string _selectedBackground = string.Empty;

    public MainViewModel(
        SettingsViewModel settings,
        PomodoroViewModel pomodoro,
        ReportViewModel report,
        HomeSettingsViewModel homeSettings,
        FocusTimeViewModel focusTime,
        NotificationsViewModel notifications,
        TagsViewModel tags,
        BackgroundViewModel background,
        IPlaySoundService playSound,
        INotificationService notificationService,
        ISessionService sessionService)
    {
        Settings = settings ?? new SettingsViewModel(homeSettings, focusTime, notifications, tags, background);
        Pomodoro = pomodoro ?? new PomodoroViewModel(Settings, playSound, notificationService, sessionService);
        Report = report ?? new ReportViewModel(sessionService);

        CurrentViewModel = Pomodoro;

        this.WhenAnyValue(vm => vm.Settings.Background.SelectedBackground)
            .DistinctUntilChanged()
            .Subscribe(b => SelectedBackground = b);

        ListenToEvents();
    }

    public SettingsViewModel Settings { get; }
    public PomodoroViewModel Pomodoro { get; }
    public ReportViewModel Report { get; }

    private void ListenToEvents()
    {
        Pomodoro.Settings.HomeSettings.OnReportCommand
            .Where(r => r == true)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Select(_ => Unit.Default)
            .InvokeCommand(NavigateToReportCommand);

        Report.OnBackCommand
            .ObserveOn(RxApp.MainThreadScheduler)
            .Select(_ => Unit.Default)
            .InvokeCommand(NavigateToPomodoroCommand);
    }

    [ReactiveCommand]
    private async Task NavigateToReport()
    {
        await Report.Reload();
        CurrentViewModel = Report;

        Log.Debug("Go to Report");
    }

    [ReactiveCommand]
    private void NavigateToPomodoro()
    {
        Pomodoro.ShowingSettings = false;
        CurrentViewModel = Pomodoro;

        Log.Debug("Go to Pomodoro");
    }
}