using EasyFocus.Common;
using EasyFocus.Domain.Entities;
using EasyFocus.Domain.Services;
using EasyFocus.Features.Settings;
using EasyFocus.Features.Settings.Tags;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace EasyFocus.Features.Pomodoro;

public sealed partial class PomodoroViewModel : ViewModelBase, IActivatableViewModel
{
#if DEBUG
    private const int _minutesToSecondsFactor = 1; // for testing: minutes = seconds.
#else
    private const int _minutesToSecondsFactor = 60;
#endif

    private bool _isSkippingSession;

    [Reactive] private bool _showingSettings;

    [Reactive] private bool _isTimerTicking;
    [Reactive] private int _secondsLeft;

    [Reactive] private SessionType _sessionType;
    [Reactive] private int _pomodorosCompleted;
    [Reactive] private int _shortBreaksCompleted;
    [Reactive] private int _longBreaksCompleted;

    [Reactive] private TagItemViewModel? _selectedTag;

    [Reactive] private bool _smallWindowMode;

    private readonly AppSettings _appSettings;
    private readonly IPlaySoundService _playSoundService;
    private readonly INotificationService _notificationService;
    private readonly ISessionService _sessionService;
    private readonly IBrowserService _browserService;

    public PomodoroViewModel(
        SettingsViewModel settings,
        IPlaySoundService playSoundService,
        INotificationService notificationService,
        ISessionService sessionService,
        IBrowserService browserService)
    {
        Settings = settings;
        _appSettings = Settings.FocusTime.Settings;
        _playSoundService = playSoundService;
        _notificationService = notificationService;
        _sessionService = sessionService;
        _browserService = browserService;

        SessionType = SessionType.Pomodoro;
        SelectedTag = Tags.FirstOrDefault();

        Observable.Interval(TimeSpan.FromSeconds(1))
            .Where(_ => IsTimerTicking)
            .Select(_ => System.Reactive.Unit.Default)
            .ObserveOn(RxApp.MainThreadScheduler)
            .InvokeCommand(TimerTickCommand);

        this.WhenAnyValue(vm => vm.Settings.FocusTime.Pomodoro)
            .Where(_ => !IsTimerTicking)
            .Where(_ => SessionType == SessionType.Pomodoro)
            .Select(p => p * _minutesToSecondsFactor)
            .Subscribe(p => SecondsLeft = p);

        this.WhenAnyValue(vm => vm.Settings.FocusTime.Break)
            .Where(_ => !IsTimerTicking)
            .Where(_ => SessionType == SessionType.ShortBreak)
            .Select(p => p * _minutesToSecondsFactor)
            .Subscribe(p => SecondsLeft = p);

        this.WhenAnyValue(vm => vm.Settings.FocusTime.LongBreak)
            .Where(_ => !IsTimerTicking)
            .Where(_ => SessionType == SessionType.LongBreak)
            .Select(p => p * _minutesToSecondsFactor)
            .Subscribe(p => SecondsLeft = p);

        this.WhenAnyValue(vm => vm.SelectedTag)
            .Where(tag => tag is null)
            .Subscribe(_ => SelectedTag = Tags.FirstOrDefault());

        this.WhenAnyValue(vm => vm.SessionType)
            .Buffer(2, 1)
            .Select(b => (previous: b[0], current: b[1]))
            .InvokeCommand(SessionChangedCommand);

        this.WhenAnyValue(vm => vm.SecondsLeft)
            .InvokeCommand(UpdateBrowserTitleCommand);

        this.WhenAnyValue(vm => vm.Settings.FocusTime.ShowTodaySession)
            .Where(s => s == true)
            .Select(_ => Observable.StartAsync(LoadTodaySessions))
            .Concat()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(sessions =>
            {
                if (_appSettings.ShowTodaySessions)
                {
                    PomodorosCompleted = sessions.Count(s => s.SessionType == SessionType.Pomodoro);
                    ShortBreaksCompleted = sessions.Count(s => s.SessionType == SessionType.ShortBreak);
                    LongBreaksCompleted = sessions.Count(s => s.SessionType == SessionType.LongBreak);
                }
            });

        this.WhenActivated(Activated);
    }

    public ObservableCollection<TagItemViewModel> Tags => Settings.Tags.Tags;
    public SettingsViewModel Settings { get; }
    public int PomodorosBeforeLongBreak => Settings.FocusTime.PomodoroSessionsBeforeLongBreak;
    public int PomodoroTotalSeconds => Settings.FocusTime.Pomodoro * _minutesToSecondsFactor;
    public int ShortBreakTotalSeconds => Settings.FocusTime.Break * _minutesToSecondsFactor;
    public int LongBreakTotalSeconds => Settings.FocusTime.LongBreak * _minutesToSecondsFactor;
    public bool AutoStartPomodoros => Settings.FocusTime.AutoStartPomodorosEnabled;
    public bool AutoStartBreaks => Settings.FocusTime.AutoStartBreaksEnabled;
    public bool NotificationOnCompletion => Settings.Notifications.NotificationOnCompletionEnabled;
    public bool NotificationWhenSkippingSession => Settings.Notifications.NotificationWhenSkippingSession;
    public bool SaveProgressWhenSkippingSession => Settings.FocusTime.SaveProgressWhenSkippingSession;

    public ViewModelActivator Activator { get; } = new();

    private void Activated(CompositeDisposable d)
    {
        Observable.StartAsync(LoadTodaySessions)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(sessions =>
            {
                if (_appSettings.ShowTodaySessions)
                {
                    PomodorosCompleted = sessions.Count(s => s.SessionType == SessionType.Pomodoro);
                    ShortBreaksCompleted = sessions.Count(s => s.SessionType == SessionType.ShortBreak);
                    LongBreaksCompleted = sessions.Count(s => s.SessionType == SessionType.LongBreak);
                }
            });
    }

    private async Task<List<Session>> LoadTodaySessions()
    {
        var sessions = await _sessionService.GetSessionsAsync();
        return sessions.Where(sessions => sessions.FinishedDateTime.Date == DateTime.Now.Date).ToList();
    }

    [ReactiveCommand]
    private async Task SessionChanged((SessionType previous, SessionType current) sessionType)
    {
        Log.Information($"Session changed from {sessionType.previous} to {sessionType.current}!");

        bool oldTimerTicking = IsTimerTicking;
        IsTimerTicking = false;

        await CompletedSession(sessionType.previous, _isSkippingSession);

        SetupSession(SessionType);

        _isSkippingSession = false;

        IsTimerTicking = oldTimerTicking;
    }

    private async Task CompletedSession(SessionType sessionType, bool isSkippingSession)
    {
        if (!isSkippingSession || SaveProgressWhenSkippingSession)
        {
            if (sessionType == SessionType.Pomodoro)
            {
                ++PomodorosCompleted;
            }
            else if (sessionType == SessionType.ShortBreak)
            {
                ++ShortBreaksCompleted;
            }
            else if (sessionType == SessionType.LongBreak)
            {
                ++LongBreaksCompleted;
            }
        }

        if (!isSkippingSession || NotificationWhenSkippingSession)
        {
            SessionCompletedNotification(sessionType);
            SessionCompletedSound();
        }

        if (!isSkippingSession || SaveProgressWhenSkippingSession)
        {
            await SaveProgress(sessionType);
        }
    }

    [ReactiveCommand]
    private void TimerTick()
    {
        if (SecondsLeft > 0)
        {
            --SecondsLeft;
            return;
        }

        Log.Information("Timer tick completed");

        _isSkippingSession = false;

        if (SessionType == SessionType.Pomodoro)
        {
            if ((PomodorosCompleted + 1) % PomodorosBeforeLongBreak == 0)
            {
                SessionType = SessionType.LongBreak;
            }
            else
            {
                SessionType = SessionType.ShortBreak;
            }
        }
        else if (SessionType == SessionType.ShortBreak)
        {
            SessionType = SessionType.Pomodoro;
        }
        else if (SessionType == SessionType.LongBreak)
        {
            SessionType = SessionType.Pomodoro;
        }
    }

    [ReactiveCommand]
    private void PomodoroSession()
    {
        _isSkippingSession = true;
        SessionType = SessionType.Pomodoro;

        Log.Information("Go to Pomodoro Session");
    }

    [ReactiveCommand]
    private void ShortBreakSession()
    {
        _isSkippingSession = true;
        SessionType = SessionType.ShortBreak;

        Log.Information("Go to short break");
    }

    [ReactiveCommand]
    private void LongBreakSession()
    {
        _isSkippingSession = true;
        SessionType = SessionType.LongBreak;

        Log.Information("Go to long break");
    }

    private void SessionCompletedSound()
    {
        var sound = Settings.Notifications.SelectedSound;
        if (sound == Sound.None)
        {
            return;
        }

        var volume = Settings.Notifications.Volume;

        _ = Task.Run(() => _playSoundService.Play(sound, volume));

        Log.Information("Play sound, volume = {volume}", volume);
    }

    [ReactiveCommand]
    private void SetupSession(SessionType sessionType)
    {
        if (IsTimerTicking)
        {
            IsTimerTicking = sessionType.IsBreak() ? AutoStartBreaks : AutoStartPomodoros;
        }

        switch (sessionType)
        {
            case SessionType.Pomodoro:
                SecondsLeft = PomodoroTotalSeconds;
                break;

            case SessionType.ShortBreak:
                SecondsLeft = ShortBreakTotalSeconds;
                break;

            case SessionType.LongBreak:
                SecondsLeft = LongBreakTotalSeconds;
                break;
        }
    }

    private void SessionCompletedNotification(SessionType sessionType)
    {
        if (!NotificationOnCompletion)
        {
            return;
        }

        string title = string.Empty;
        string msg = string.Empty;

        switch (sessionType)
        {
            case SessionType.Pomodoro:
                title = "Pomodoro completed";
                msg = "Time for a break!";
                break;

            case SessionType.ShortBreak:
                title = "Break completed";
                msg = "Time to focus!";
                break;

            case SessionType.LongBreak:
                title = "Long break completed";
                msg = "Time to focus!";
                break;
        }

        _ = Task.Run(() => _notificationService.ShowNotification(title, msg));

        Log.Information("Show notification: {title}, {msg}", title, msg);
    }

    [ReactiveCommand]
    private void Restart()
    {
        IsTimerTicking = false;
        PomodorosCompleted = 0;
        ShortBreaksCompleted = 0;
        LongBreaksCompleted = 0;
        SetupSession(SessionType);

        Log.Information("Restart Pomodoros");
    }

    [ReactiveCommand]
    private async Task UpdateBrowserTitle(int secondsLeft)
    {
        await _browserService.UpdateTitleAsync(secondsLeft, IsTimerTicking);
    }

    private async Task SaveProgress(SessionType sessionType)
    {
        var durationSeconds = sessionType switch
        {
            SessionType.Pomodoro => PomodoroTotalSeconds,
            SessionType.ShortBreak => ShortBreakTotalSeconds,
            SessionType.LongBreak => LongBreakTotalSeconds,
            _ => throw new NotImplementedException()
        };

        int completedSeconds = durationSeconds - SecondsLeft;

        if (completedSeconds < 3)
        {
            return;
        }

        if (SelectedTag is null)
        {
            return;
        }

        var session = Session.CreateSession(durationSeconds, completedSeconds, sessionType, SelectedTag.Tag);
        await _sessionService.AddAsync(session);

        Log.Information("Session saved: {session}", session.DurationSeconds);
    }
}