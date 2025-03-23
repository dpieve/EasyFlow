using EasyFlow.Common;
using EasyFlow.Domain.Entities;
using EasyFlow.Domain.Services;
using EasyFlow.Features.Pomodoro;
using EasyFlow.Features.Report;
using EasyFlow.Features.Settings;
using EasyFlow.Features.Settings.Background;
using EasyFlow.Features.Settings.FocusTime;
using EasyFlow.Features.Settings.HomeSettings;
using EasyFlow.Features.Settings.Notifications;
using EasyFlow.Features.Settings.Tags;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace EasyFlow.Features;

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

        // CurrentViewModel = Report;
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
            .Subscribe(_ =>
            {
                Pomodoro.ShowingSettings = false;
                CurrentViewModel = Pomodoro;
            });
    }

    [ReactiveCommand]
    private async Task NavigateToReport()
    {
        await Report.Reload();
        CurrentViewModel = Report;
    }
}