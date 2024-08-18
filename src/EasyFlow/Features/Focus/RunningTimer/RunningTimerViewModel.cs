using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyFlow.Common;
using EasyFlow.Data;
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

public sealed partial class RunningTimerViewModel : ViewModelBase, IRoute, IActivatableRoute
{
    private readonly ITagService _tagService;
    private readonly IGeneralSettingsService _generalSettingsService;
    private readonly IPlaySoundService _playSound;
    private CompositeDisposable? _disposables;

    private GeneralSettings? _generalSettings;
    
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
    private string _timerText = string.Empty;

    [ObservableProperty]
    private double _progressValue = 100;

    [ObservableProperty]
    private string _selectedTagName = string.Empty;

    public RunningTimerViewModel(
        IRouterHost routerHost,
        ITagService? tagService = null,
        IGeneralSettingsService? generalSettingsService = null,
        IPlaySoundService? playSoundService = null)
    {
        RouterHost = routerHost;
        _tagService = tagService ?? throw new ArgumentNullException(nameof(tagService));
        _generalSettingsService = generalSettingsService ?? throw new ArgumentNullException(nameof(generalSettingsService));
        _playSound = playSoundService ?? throw new ArgumentNullException(nameof(playSoundService));

        LoadSettings();

        this.WhenAnyValue(vm => vm.SecondsLeft)
            .DistinctUntilChanged()
            .Subscribe(secondsLeft =>
            {
                Debug.WriteLine($"Seconds Left: {secondsLeft}");
                var minutes = secondsLeft / 60;
                var seconds = secondsLeft % 60;
                TimerText = $"{minutes:D2}:{seconds:D2}";

                ProgressValue = (double)secondsLeft / TotalSeconds * 100;
            });

        this.WhenAnyValue(vm => vm.TimerState)
            .Subscribe(OnStateChanged);
    }


    public string RouteName => nameof(RunningTimerViewModel);

    public IRouterHost RouterHost { get; }

    public string SkipButtonText => IsBreak ? "Skip to Focus" : "Skip to Break";

    public string ProgressText => $"{CompletedTimers}/{TimersBeforeLongBreak}";

    public MaterialIconKind StartButtonIcon => IsRunning ? MaterialIconKind.Pause : MaterialIconKind.Play;

    void IActivatableRoute.OnActivated()
    {
        Debug.WriteLine("Activated RunTimer");

        _disposables ??= [];

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
            .DisposeWith(_disposables);
    }

    void IActivatableRoute.OnDeactivated()
    {
        Debug.WriteLine("Deactivated RunTimer");

        _disposables?.Dispose();
        _disposables = null;
    }


    private void LoadSettings()
    {
        var result = _generalSettingsService.Get();
        if (result.Error is not null)
        {
            return;
        }

        var settings = result.Value!;
        _generalSettings = settings;

        TimersBeforeLongBreak = settings.WorkSessionsBeforeLongBreak;

        var totalMinutes = settings.WorkDurationMinutes;
        TotalSeconds = totalMinutes * 60;

        SecondsLeft = TotalSeconds;

        var minutes = TotalSeconds / 60;
        var seconds = TotalSeconds % 60;
        TimerText = $"{minutes:D2}:{seconds:D2}";

        var getTagResult = _generalSettingsService.GetSelectedTag();
        if (getTagResult.Error is not null)
        {
            return;
        }
        
        var selectedTag = getTagResult.Value!;

        SelectedTagName = selectedTag.Name;
    }

    private void OnStateChanged(TimerState state)
    {
        var settings = _generalSettings;
        if (settings is null)
        {
            return;
        }

        var totalMinutes = state switch
        {
            TimerState.Focus => settings.WorkDurationMinutes,
            TimerState.Break => settings.BreakDurationMinutes,
            TimerState.LongBreak => settings.LongBreakDurationMinutes,
            _ => throw new ArgumentOutOfRangeException(nameof(state))
        };

        TotalSeconds = totalMinutes * 60;
        SecondsLeft = TotalSeconds;

        IsBreak = state != TimerState.Focus;

        IsRunning = true;
    }

    [RelayCommand]
    private void EndSession() => RouterHost.Router.NavigateTo(new AdjustTimersViewModel(RouterHost, _tagService, _generalSettingsService));

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
    }

    [RelayCommand]
    private void RestartTimer()
    {
        OnStateChanged(TimerState);
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