using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyFlow.Common;
using EasyFlow.Features.Focus.AdjustTimers;
using EasyFlow.Services;
using Material.Icons;
using ReactiveUI;
using SimpleRouter;
using SukiUI.Controls;
using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace EasyFlow.Features.Focus.RunningTimer;

public enum TimerState
{
    Focus,
    Break,
    LongBreak,
}

public sealed record TimerInterval(int FocusMinutes, int BreakMinutes, int LongBreakMinutes);

public sealed partial class RunningTimerViewModel : ViewModelBase, IRoute, IActivatableRoute
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ProgressText))]
    private int _completedTimers = 0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsBreak))]
    private TimerState _timerState = TimerState.Focus;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ProgressText))]
    private int _timersBeforeLongBreak;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SkipButtonText))]
    private bool _isBreak = false;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StartButtonIcon))]
    private bool _isRunning = true;

    [ObservableProperty]
    private int _totalSeconds;

    [ObservableProperty]
    private int _secondsLeft;

    [ObservableProperty]
    private string _timerText;

    [ObservableProperty]
    private double _progressValue = 100;

    [ObservableProperty]
    private string _selectedTagName;

    private readonly FocusSettings _focusSettings;

    private readonly IPlaySoundService _playSound;

    public RunningTimerViewModel(IRouterHost routerHost, FocusSettings focusSettings)
    {
        _playSound = new PlaySoundService();

        _focusSettings = focusSettings;
        _timersBeforeLongBreak = _focusSettings.TimersBeforeLongBreak;
        RouterHost = routerHost;

        var totalMinutes = _focusSettings.WorkTime.TotalMinutes;
        TotalSeconds = totalMinutes * 60;

        SecondsLeft = TotalSeconds;

        var minutes = TotalSeconds / 60;
        var seconds = TotalSeconds % 60;
        TimerText = $"{minutes:D2}:{seconds:D2}";

        SelectedTagName = _focusSettings.Tag.Name;

        this.WhenAnyValue(vm => vm.SecondsLeft)
            .DistinctUntilChanged()
            .Subscribe(secondsLeft =>
            {
                Debug.WriteLine($"Seconds Left: {secondsLeft}");
                minutes = secondsLeft / 60;
                seconds = secondsLeft % 60;
                TimerText = $"{minutes:D2}:{seconds:D2}";

                ProgressValue = (double)secondsLeft / TotalSeconds * 100;
            });

        this.WhenAnyValue(vm => vm.TimerState)
            .Subscribe(state =>
            {
                OnStateChanged(state);
            });
    }

    private void OnStateChanged(TimerState state)
    {
        if (state == TimerState.Focus)
        {
            var totalMinutes = _focusSettings.WorkTime.TotalMinutes;
            TotalSeconds = totalMinutes * 60;

            SecondsLeft = TotalSeconds;
        }
        else if (state == TimerState.Break)
        {
            var totalMinutes = _focusSettings.BreakTime.TotalMinutes;
            TotalSeconds = totalMinutes * 60;

            SecondsLeft = TotalSeconds;
        }
        else if (state == TimerState.LongBreak)
        {
            var totalMinutes = _focusSettings.LongBreakTime.TotalMinutes;
            TotalSeconds = totalMinutes * 60;

            SecondsLeft = TotalSeconds;
        }

        // TEST
        SecondsLeft = 5;

        if (state == TimerState.Focus)
        {
            IsBreak = false;
        }
        else
        {
            IsBreak = true;
        }

        IsRunning = true;
    }

    void IActivatableRoute.OnActivated()
    {
        Debug.WriteLine("Activated RunTimer");

        Disposables ??= new();

        Observable.Interval(TimeSpan.FromSeconds(1))
            .Where(_ => IsRunning)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ =>
            {
                if (SecondsLeft == 0)
                {
                    IsRunning = false;

                    if (TimerState == TimerState.Focus)
                    {
                        _playSound.Play(SoundType.Break);

                        SukiHost.ShowToast("Focus Completed", "Well done! You can rest now.", SukiUI.Enums.NotificationType.Success);
                    }
                    else if (TimerState == TimerState.Break)
                    {
                        _playSound.Play(SoundType.Work);

                        SukiHost.ShowToast("Break Completed", "Time to focus!", SukiUI.Enums.NotificationType.Success);
                    }
                    else if (TimerState == TimerState.LongBreak)
                    {
                        _playSound.Play(SoundType.Work);

                        SukiHost.ShowToast("Long Break Completed", "Enough rest, time to focus!", SukiUI.Enums.NotificationType.Success);
                    }

                    GoToNextState();
                }
                if (SecondsLeft > 0)
                {
                    SecondsLeft--;
                }
            })
            .DisposeWith(Disposables);
    }

    void IActivatableRoute.OnDeactivated()
    {
        Debug.WriteLine("Deactivated RunTimer");

        Disposables?.Dispose();
        Disposables = null;
    }

    public CompositeDisposable? Disposables { get; set; }

    public string RouteName => nameof(RunningTimerViewModel);

    public IRouterHost RouterHost { get; }

    public string SkipButtonText => IsBreak ? "Skip to Focus" : "Skip to Break";

    public string ProgressText => $"{CompletedTimers}/{TimersBeforeLongBreak}";

    public MaterialIconKind StartButtonIcon => IsRunning ? MaterialIconKind.Pause : MaterialIconKind.Play;

    [RelayCommand]
    private void NavigateToSetupTimer() => RouterHost.Router.NavigateTo(new AdjustTimersViewModel(RouterHost));

    [RelayCommand]
    private void SkipToBreak()
    {
        GoToNextState();

        Debug.WriteLine($"Timer State = {TimerState}");
    }

    [RelayCommand]
    private void StartOrPauseTimer()
    {
        IsRunning = !IsRunning;

        var msg = IsRunning ? "Started" : "Paused";
        SukiHost.ShowToast($"Timer {msg}", $"The timer was {msg}.");
    }

    [RelayCommand]
    private void RestartTimer()
    {
        OnStateChanged(TimerState);
        SukiHost.ShowToast("Timer Restarted", "The timer was restarted.");
    }

    private void GoToNextState()
    {
        if (TimerState == TimerState.LongBreak)
        {
            CompletedTimers = 0;
        }

        if (TimerState == TimerState.Focus)
        {
            ++CompletedTimers;
            if (CompletedTimers == TimersBeforeLongBreak)
            {
                TimerState = TimerState.LongBreak;
            }
            else
            {
                TimerState = TimerState.Break;
            }
        }
        else
        {
            TimerState = TimerState.Focus;
        }
    }
}