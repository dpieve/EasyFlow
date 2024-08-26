//using Avalonia;
//using CommunityToolkit.Mvvm.ComponentModel;
//using CommunityToolkit.Mvvm.Input;
//using EasyFlow.Presentation.Common;
//using EasyFlow.Presentation.Data;
//using EasyFlow.Presentation.Features.Focus.AdjustTimers;
//using EasyFlow.Presentation.Services;
//using Material.Icons;
//using ReactiveUI;
//using SimpleRouter;
//using SukiUI.Controls;
//using System;
////using System.Diagnostics;
//using System.Reactive;
//using System.Reactive.Disposables;
//using System.Reactive.Linq;
//using System.Threading.Tasks;

//namespace EasyFlow.Presentation.Features.Focus.RunningTimer;

//public sealed partial class RunningTimerViewModel : ViewModelBase, IRoute, IActivatableRoute
//{
//    private readonly ITagService _tagService;
//    private readonly IGeneralSettingsService _generalSettingsService;
//    private readonly IPlaySoundService _playSound;
//    private readonly ISessionService _sessionService;

//    private CompositeDisposable? _disposables;

//    [ObservableProperty]
//    [NotifyPropertyChangedFor(nameof(ProgressText))]
//    private int _completedTimers = 0;

//    [ObservableProperty]
//    [NotifyPropertyChangedFor(nameof(IsBreak))]
//    private TimerState _timerState = TimerState.Focus;

//    [ObservableProperty]
//    [NotifyPropertyChangedFor(nameof(ProgressText))]
//    private int _timersBeforeLongBreak;

//    [ObservableProperty]
//    [NotifyPropertyChangedFor(nameof(SkipButtonText))]
//    private bool _isBreak = false;

//    [ObservableProperty]
//    [NotifyPropertyChangedFor(nameof(StartButtonIcon))]
//    private bool _isRunning;

//    [ObservableProperty]
//    private int _totalSeconds;

//    [ObservableProperty]
//    private int _secondsLeft;

//    [ObservableProperty]
//    private string _timerText = string.Empty;

//    [ObservableProperty]
//    private double _progressValue = 100;

//    [ObservableProperty]
//    private string _selectedTagName = string.Empty;

//    [ObservableProperty]
//    private bool _isFocusDescriptionVisible = true;

//    [ObservableProperty]
//    private string _description = string.Empty;

//    public RunningTimerViewModel(
//        IRouterHost routerHost,
//        ITagService? tagService = null,
//        IGeneralSettingsService? generalSettingsService = null,
//        IPlaySoundService? playSoundService = null,
//        ISessionService? sessionService = null)
//    {
//        RouterHost = routerHost;
//        _tagService = tagService ?? throw new ArgumentNullException(nameof(tagService));
//        _generalSettingsService = generalSettingsService ?? throw new ArgumentNullException(nameof(generalSettingsService));
//        _playSound = playSoundService ?? throw new ArgumentNullException(nameof(playSoundService));
//        _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));

//        this.WhenAnyValue(vm => vm.SecondsLeft)
//            .DistinctUntilChanged()
//            .Subscribe(secondsLeft =>
//            {
//                Debug.WriteLine($"Seconds Left: {secondsLeft}");
//                var minutes = secondsLeft / 60;
//                var seconds = secondsLeft % 60;
//                TimerText = $"{minutes:D2}:{seconds:D2}";

//                ProgressValue = (double)secondsLeft / TotalSeconds * 100;
//            });

//        this.WhenAnyValue(vm => vm.TimerState)
//            .Skip(1)
//            .Subscribe(OnStateChanged);

//        this.WhenAnyValue(vm => vm.TimerState)
//            .InvokeCommand(UpdateNotesVisibleCommand);
//    }

//    public string RouteName => nameof(RunningTimerViewModel);

//    public IRouterHost RouterHost { get; }

//    public string SkipButtonText => IsBreak ? "Skip to Focus" : "Skip to Break";

//    public string ProgressText => $"{CompletedTimers}/{TimersBeforeLongBreak}";

//    public MaterialIconKind StartButtonIcon => IsRunning ? MaterialIconKind.Pause : MaterialIconKind.Play;

//    void IActivatableRoute.OnActivated()
//    {
//        Debug.WriteLine("Activated RunTimer");

//        _disposables ??= [];

//        LoadSettings();

//        IsRunning = true;

//        Observable.Interval(TimeSpan.FromSeconds(1))
//            .Where(_ => IsRunning)
//            .Select(_ => Unit.Default)
//            .ObserveOn(RxApp.MainThreadScheduler)
//            .InvokeCommand(TimerTickCommand)
//            .DisposeWith(_disposables);
//    }

//    void IActivatableRoute.OnDeactivated()
//    {
//        Debug.WriteLine("Deactivated RunTimer");

//        _disposables?.Dispose();
//        _disposables = null;
//    }

//    private void LoadSettings()
//    {
//        var result = _generalSettingsService.Get();
//        if (result.Error is not null)
//        {
//            return;
//        }

//        var settings = result.Value!;

//        IsFocusDescriptionVisible = settings.IsFocusDescriptionEnabled;
//        TimersBeforeLongBreak = settings.WorkSessionsBeforeLongBreak;

//        var getTagResult = _generalSettingsService.GetSelectedTag();
//        if (getTagResult.Error is not null)
//        {
//            return;
//        }

//        var selectedTag = getTagResult.Value!;
//        SelectedTagName = selectedTag.Name;

//        if (IsRunning)
//        {
//            return;
//        }

//        var totalMinutes = settings.WorkDurationMinutes;
//        TotalSeconds = totalMinutes * 60;

//        var minutes = TotalSeconds / 60;
//        var seconds = TotalSeconds % 60;
//        TimerText = $"{minutes:D2}:{seconds:D2}";

//        SecondsLeft = TotalSeconds;

//        // Test
//        SecondsLeft = 4;
//    }

//    private void OnStateChanged(TimerState state)
//    {
//        var result = _generalSettingsService.Get();
//        if (result.Error is not null)
//        {
//            return;
//        }

//        Description = string.Empty;

//        var settings = result.Value!;

//        var totalMinutes = state switch
//        {
//            TimerState.Focus => settings.WorkDurationMinutes,
//            TimerState.Break => settings.BreakDurationMinutes,
//            TimerState.LongBreak => settings.LongBreakDurationMinutes,
//            _ => throw new ArgumentOutOfRangeException(nameof(state))
//        };

//        TotalSeconds = totalMinutes * 60;
//        SecondsLeft = TotalSeconds;

//        // TEST:
//        SecondsLeft = 3;

//        IsBreak = state != TimerState.Focus;
//    }

//    [RelayCommand]
//    private async Task TimerTick()
//    {
//        if (SecondsLeft > 0)
//        {
//            SecondsLeft--;
//        }
//        else if (SecondsLeft == 0)
//        {
//            if (TimerState == TimerState.Focus)
//            {
//                _playSound.Play(SoundType.Break);

//                await SukiHost.ShowToast("Focus Completed", "Well done! You can rest now.", SukiUI.Enums.NotificationType.Success);
//            }
//            else if (TimerState == TimerState.Break)
//            {
//                _playSound.Play(SoundType.Work);

//                await SukiHost.ShowToast("Break Completed", "Time to focus!", SukiUI.Enums.NotificationType.Success);
//            }
//            else if (TimerState == TimerState.LongBreak)
//            {
//                _playSound.Play(SoundType.Work);

//                await SukiHost.ShowToast("Long Break Completed", "Enough rest, time to focus!", SukiUI.Enums.NotificationType.Success);
//            }

//            await GoToNextState(isSkipping: false);
//        }
//    }

//    [RelayCommand]
//    private void EndSession() => RouterHost.Router.NavigateTo(new AdjustTimersViewModel(RouterHost, _tagService, _generalSettingsService));

//    [RelayCommand]
//    private async Task SkipToBreak()
//    {
//        await GoToNextState(isSkipping: true);

//        Debug.WriteLine($"Timer State = {TimerState}");
//    }

//    [RelayCommand]
//    private void StartOrPauseTimer()
//    {
//        IsRunning = !IsRunning;
//    }

//    [RelayCommand]
//    private void RestartTimer()
//    {
//        OnStateChanged(TimerState);
//    }

//    [RelayCommand]
//    private void OpenNotes(Session? session = null)
//    {
//        var previousRunningState = IsRunning;
//        IsRunning = false;

//        var isDescriptionEnabled = _generalSettingsService.IsFocusDescriptionEnabled();
//        if (isDescriptionEnabled)
//        {
//            SukiHost.ShowDialog(new EditDescriptionViewModel(Description, (string notes) =>
//            {
//                Description = notes;
//                IsRunning = previousRunningState;
            
//                if (session is not null)
//                {
//                    session.Description = notes;
//                    _sessionService.UpdateAsync(session).GetAwaiter().GetResult();
//                }
//            },
//            () => IsRunning = previousRunningState),
//            allowBackgroundClose: false);
//        }
//    }

//    [RelayCommand]
//    private void UpdateNotesVisible()
//    {
//        var result = _generalSettingsService.Get();
//        if (result.Error is not null)
//        {
//            return;
//        }

//        var settings = result.Value!;
//        IsFocusDescriptionVisible = settings.IsFocusDescriptionEnabled && TimerState == TimerState.Focus;
//    }

//    private async Task GoToNextState(bool isSkipping)
//    {
//        if (!isSkipping)
//        {
//            var resultSettings = _generalSettingsService.Get();
//            if (resultSettings.Error is not null)
//            {
//                return;
//            }

//            var settings = resultSettings.Value!;

//            var sessionsType = TimerState switch
//            {
//                TimerState.Focus => SessionType.Focus,
//                TimerState.Break => SessionType.Break,
//                TimerState.LongBreak => SessionType.LongBreak,
//                _ => SessionType.Focus
//            };

//            var duration = TimerState switch
//            {
//                TimerState.Focus => settings!.WorkDurationMinutes,
//                TimerState.Break => settings!.BreakDurationMinutes,
//                TimerState.LongBreak => settings!.LongBreakDurationMinutes,
//                _ => settings!.WorkDurationMinutes
//            };


//            var currentDate = DateTime.Now;
//            DateTime newDateTime = new(currentDate.Year, currentDate.Month, currentDate.Day, 0, 0, 0, DateTimeKind.Utc);

//            var session = new Session
//            {
//                DurationMinutes = duration,
//                SessionType = sessionsType,
//                FinishedDate = newDateTime,
//                TagId = settings!.SelectedTagId,
//                Tag = settings.SelectedTag,
//                Description = sessionsType != SessionType.Focus ? " - " : Description
//            };

//            var result = await _sessionService.CreateAsync(session);
//            if (result.Error is not null)
//            {
//                await SukiHost.ShowToast("Failed to save session", "Failed to save the session to the database", SukiUI.Enums.NotificationType.Error);
//                return;
//            }

//            var isDescriptionEnabled = _generalSettingsService.IsFocusDescriptionEnabled();
//            if (isDescriptionEnabled && TimerState == TimerState.Focus)
//            {
//                OpenNotes(session);
//            }
//        }

//        if (TimerState == TimerState.LongBreak)
//        {
//            CompletedTimers = 0;
//        }

//        if (TimerState == TimerState.Focus)
//        {
//            ++CompletedTimers;
//            if (CompletedTimers == TimersBeforeLongBreak)
//            {
//                TimerState = TimerState.LongBreak;
//            }
//            else
//            {
//                TimerState = TimerState.Break;
//            }
//        }
//        else
//        {
//            TimerState = TimerState.Focus;
//        }
//    }
//}