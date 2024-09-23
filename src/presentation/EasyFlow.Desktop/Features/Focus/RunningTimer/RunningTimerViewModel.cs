using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyFlow.Application.Services;
using EasyFlow.Application.Sessions;
using EasyFlow.Application.Settings;
using EasyFlow.Desktop.Common;
using EasyFlow.Desktop.Features.Focus.RunningTimer;
using EasyFlow.Desktop.Services;
using EasyFlow.Domain.Entities;
using EasyFlow.Desktop.Features.Focus.AdjustTimers;
using Material.Icons;
using MediatR;
using ReactiveUI;
using Serilog;
using SimpleRouter;
using SukiUI.Dialogs;
using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace EasyFlow.Desktop.Features.Focus.RunningTimer;

public sealed partial class RunningTimerViewModel : ViewModelBase, IRoute, IActivatableRoute
{
    private readonly IMediator _mediator;
    private readonly ILanguageService _languageService;
    private readonly IToastService _toastService;
    private readonly ISukiDialogManager _dialog;
    private readonly INotificationService _notificationService;

    private CompositeDisposable? _disposables;

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
    private bool _isRunning;

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

    [ObservableProperty]
    private bool _isFocusDescriptionVisible = true;

    [ObservableProperty]
    private string _description = string.Empty;

    public RunningTimerViewModel(
        IRouterHost routerHost,
        IMediator mediator,
        ILanguageService languageService,
        IToastService toastService,
        ISukiDialogManager dialog,
        INotificationService notificationService)
    {
        RouterHost = routerHost;
        _mediator = mediator;
        _languageService = languageService;
        _toastService = toastService;
        _dialog = dialog;
        _notificationService = notificationService;
        
        this.WhenAnyValue(vm => vm.SecondsLeft)
            .DistinctUntilChanged()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(secondsLeft =>
            {
                var minutes = secondsLeft / 60;
                var seconds = secondsLeft % 60;
                TimerText = $"{minutes:D2}:{seconds:D2}";
                ProgressValue = (double)secondsLeft / TotalSeconds * 100;
            });

        this.WhenAnyValue(vm => vm.TimerState)
            .Skip(1)
            .ObserveOn(RxApp.MainThreadScheduler)
            .InvokeCommand(StateChangedCommand);

        this.WhenAnyValue(vm => vm.TimerState)
            .ObserveOn(RxApp.MainThreadScheduler)
            .InvokeCommand(UpdateNotesVisibleCommand);
    }

    public string RouteName => nameof(RunningTimerViewModel);

    public IRouterHost RouterHost { get; }

    public string SkipButtonText => IsBreak ? ConstantTranslation.SkipToFocus : ConstantTranslation.SkipToBreak;

    public string ProgressText => $"{CompletedTimers}/{TimersBeforeLongBreak}";

    public MaterialIconKind StartButtonIcon => IsRunning ? MaterialIconKind.Pause : MaterialIconKind.Play;

    void IActivatableRoute.OnActivated()
    {
        _disposables ??= [];

        Observable
            .StartAsync(GetSettings)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(settings =>
            {
                IsFocusDescriptionVisible = settings.IsFocusDescriptionEnabled;
                TimersBeforeLongBreak = settings.WorkSessionsBeforeLongBreak;
                SelectedTagName = settings.SelectedTag?.Name ?? string.Empty;

                if (IsRunning)
                {
                    return;
                }

                if (TotalSeconds == 0)
                {
                    var totalMinutes = settings.WorkDurationMinutes;
                    TotalSeconds = totalMinutes * 60;

                    var minutes = TotalSeconds / 60;
                    var seconds = TotalSeconds % 60;
                    TimerText = $"{minutes:D2}:{seconds:D2}";

                    SecondsLeft = TotalSeconds;

                    IsRunning = true;
                }
            })
            .DisposeWith(_disposables);

        Observable.Interval(TimeSpan.FromSeconds(1))
            .Where(_ => IsRunning)
            .Select(_ => System.Reactive.Unit.Default)
            .ObserveOn(RxApp.MainThreadScheduler)
            .InvokeCommand(TimerTickCommand)
            .DisposeWith(_disposables);

        Trace.TraceInformation("RunningTimer Activated");
    }

    void IActivatableRoute.OnDeactivated()
    {
        _disposables?.Dispose();
        _disposables = null;

        Trace.TraceInformation("RunningTimer Deactivated");
    }

    [RelayCommand]
    private async Task TimerTick()
    {
        try
        {
            Trace.TraceInformation($"TimerTick, SecondsLeft = {SecondsLeft}");

            if (SecondsLeft > 0)
            {
                SecondsLeft--;
            }
            else if (SecondsLeft == 0)
            {
                if (TimerState == TimerState.Focus)
                {
                    await _mediator.Send(new PlaySoundQuery() { SoundType = SoundType.Break });
                    await _notificationService.Show(_languageService.GetString("Success"), _languageService.GetString("FocusCompleted"));
                    _toastService.Display(_languageService.GetString("Success"), _languageService.GetString("FocusCompleted"), NotificationType.Success);
                }
                else if (TimerState == TimerState.Break)
                {
                    await _mediator.Send(new PlaySoundQuery() { SoundType = SoundType.Work });
                    await _notificationService.Show(_languageService.GetString("Information"), _languageService.GetString("BreakCompleted"));
                    _toastService.Display(_languageService.GetString("Information"), _languageService.GetString("BreakCompleted"), NotificationType.Information);
                }
                else if (TimerState == TimerState.LongBreak)
                {
                    await _mediator.Send(new PlaySoundQuery() { SoundType = SoundType.Work });
                    await _notificationService.Show(_languageService.GetString("Information"), _languageService.GetString("LongBreakCompleted"));
                    _toastService.Display(_languageService.GetString("Information"), _languageService.GetString("LongBreakCompleted"), NotificationType.Information);
                }

                await GoToNextState(isSkipping: false);
            }
        }
        catch(Exception ex)
        {
            Log.Error("Failed to tick timer {Error}", ex.Message);
            Restart();
        }
    }

    [RelayCommand]
    private async Task EndSession()
    {
        var result = await _mediator.Send(new GetSettingsQuery());
        RouterHost.Router.NavigateTo(new AdjustTimersViewModel(result.Value, RouterHost, _mediator, _languageService, _toastService, _dialog, _notificationService));

        Trace.TraceInformation("EndSession");
    }

    [RelayCommand]
    private async Task SkipToBreak()
    {
        await GoToNextState(isSkipping: true);
        Trace.TraceInformation("SkipToBreak");
    }

    [RelayCommand]
    private void StartOrPauseTimer()
    {
        IsRunning = !IsRunning;
    }

    [RelayCommand]
    private async Task RestartTimer()
    {
        await StateChanged(TimerState);
    }

    [RelayCommand]
    private async Task OpenNotes(Session? session = null)
    {
        try
        { 
            var previousRunningState = IsRunning;
            IsRunning = false;

            var result = await _mediator.Send(new GetSettingsQuery());
            var settings = result.Value;

            var isDescriptionEnabled = settings.IsFocusDescriptionEnabled;
            if (isDescriptionEnabled)
            {
                _dialog.CreateDialog()
                    .WithViewModel(dialog => new EditDescriptionViewModel(dialog, Description, notes =>
                    {
                        Description = notes;
                        IsRunning = previousRunningState;

                        if (session is not null)
                        {
                            session.Description = notes;
                            _mediator.Send(new CreateSessionCommand() { Session = session }).GetAwaiter().GetResult();
                        }
                    },
                    () => IsRunning = previousRunningState))
                    .TryShow();
            }
        }
        catch(Exception ex)
        {
            Log.Error("Failed to open notes, {Error}", ex.Message);
        }

        Trace.TraceInformation("OpenNotes");
    }

    [RelayCommand]
    private async Task UpdateNotesVisible()
    {
        var settings = await GetSettings();
        IsFocusDescriptionVisible = settings.IsFocusDescriptionEnabled && TimerState == TimerState.Focus;
    }

    private async Task GoToNextState(bool isSkipping)
    {
        if (!isSkipping)
        {
            var settings = await GetSettings();

            var sessionsType = TimerState switch
            {
                TimerState.Focus => SessionType.Focus,
                TimerState.Break => SessionType.Break,
                TimerState.LongBreak => SessionType.LongBreak,
                _ => SessionType.Focus
            };

            var duration = TimerState switch
            {
                TimerState.Focus => settings!.WorkDurationMinutes,
                TimerState.Break => settings!.BreakDurationMinutes,
                TimerState.LongBreak => settings!.LongBreakDurationMinutes,
                _ => settings!.WorkDurationMinutes
            };

            var currentDate = DateTime.Now;

            var session = new Session
            {
                DurationMinutes = duration,
                SessionType = sessionsType,
                FinishedDate = currentDate,
                TagId = settings!.SelectedTagId,
                Tag = settings.SelectedTag,
                Description = sessionsType != SessionType.Focus ? " - " : Description
            };

            await _mediator.Send(new CreateSessionCommand() { Session = session });

            var isDescriptionEnabled = settings.IsFocusDescriptionEnabled;
            if (isDescriptionEnabled && TimerState == TimerState.Focus)
            {
                await OpenNotes(session);
            }
        }

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

    private async Task<GeneralSettings> GetSettings()
    {
        var result = await _mediator.Send(new GetSettingsQuery());
        if (!result.IsSuccess)
        {
            Log.Warning("Failed to get settings {Error}", result.Error);
            return new();
        }
        var settings = result.Value;
        return settings;
    }

    [RelayCommand]
    private async Task StateChanged(TimerState state)
    {
        var settings = await GetSettings();

        Description = string.Empty;

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
    }

    private void Restart()
    {
        Trace.TraceInformation("Restarting");

        _disposables?.Dispose();
        _disposables = null;

        _disposables = [];

        Observable.Interval(TimeSpan.FromSeconds(1))
           .Where(_ => IsRunning)
           .Select(_ => System.Reactive.Unit.Default)
           .ObserveOn(RxApp.MainThreadScheduler)
           .InvokeCommand(TimerTickCommand)
           .DisposeWith(_disposables);
    }
}