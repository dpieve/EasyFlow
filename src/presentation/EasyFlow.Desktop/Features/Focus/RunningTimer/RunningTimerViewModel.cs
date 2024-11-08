using Avalonia.Controls.Notifications;
using EasyFlow.Application.Services;
using EasyFlow.Desktop.Common;
using EasyFlow.Desktop.Features.Focus.AdjustTimers;
using EasyFlow.Desktop.Services;
using EasyFlow.Domain.Entities;
using Material.Icons;
using MediatR;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Serilog;
using SukiUI.Dialogs;
using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace EasyFlow.Desktop.Features.Focus.RunningTimer;

public sealed partial class RunningTimerViewModel : ActivatablePageViewModelBase
{
    private readonly IMediator _mediator;
    private readonly ILanguageService _languageService;
    private readonly IToastService _toastService;
    private readonly ISukiDialogManager _dialog;
    private readonly INotificationService _notificationService;

    private CompositeDisposable? _disposables;

    [Reactive] private int _completedTimers = 0;
    [Reactive] private TimerState _timerState = TimerState.Focus;
    [Reactive] private int _timersBeforeLongBreak;
    [Reactive] private bool _isBreak = false;
    [Reactive] private bool _isRunning;
    [Reactive] private int _totalSeconds;
    [Reactive] private int _secondsLeft;
    [Reactive] private string _timerText = string.Empty;
    [Reactive] private double _progressValue = 100;
    [Reactive] private string _selectedTagName = string.Empty;
    [Reactive] private bool _isFocusDescriptionVisible = true;
    [Reactive] private string _description = string.Empty;

    [ObservableAsProperty] private string _progressText = string.Empty;
    [ObservableAsProperty] private string _skipButtonText = string.Empty;
    [ObservableAsProperty] private MaterialIconKind _startButtonIcon = MaterialIconKind.Play;

    public RunningTimerViewModel(
        IMediator mediator,
        ILanguageService languageService,
        IToastService toastService,
        ISukiDialogManager dialog,
        INotificationService notificationService,
        IScreen hostScreen,
        string urlPath = nameof(RunningTimerViewModel))
            : base(hostScreen, urlPath)
    {
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
            .Select(_ => System.Reactive.Unit.Default)
            .InvokeCommand(UpdateNotesVisibleCommand);

        _progressTextHelper = this.WhenAnyValue(vm => vm.TimersBeforeLongBreak)
            .Select(timersBeforeLongBreak => $"{CompletedTimers}/{timersBeforeLongBreak}")
            .ToProperty(this, vm => vm.ProgressText);

        _progressTextHelper = this.WhenAnyValue(vm => vm.CompletedTimers)
            .Select(completedTimers => $"{completedTimers}/{TimersBeforeLongBreak}")
            .ToProperty(this, vm => vm.ProgressText);

        _skipButtonTextHelper = this.WhenAnyValue(vm => vm.IsBreak)
            .Select(_ => IsBreak ? ConstantTranslation.SkipToFocus : ConstantTranslation.SkipToBreak)
            .ToProperty(this, vm => vm.SkipButtonText);

        _startButtonIconHelper = this.WhenAnyValue(vm => vm.IsRunning)
            .Select(_ => IsRunning ? MaterialIconKind.Pause : MaterialIconKind.Play)
            .ToProperty(this, vm => vm.StartButtonIcon);
    }

    public override void HandleActivation(CompositeDisposable d)
    {
        _disposables ??= [];

        Observable
            .StartAsync(GetSettings)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(settings =>
            {
                IsFocusDescriptionVisible = settings.IsFocusDescriptionEnabled && TimerState == TimerState.Focus;
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
#if DEBUG
                    SecondsLeft = 5;
#endif

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

    public override void HandleDeactivation()
    {
        _disposables?.Dispose();
        _disposables = null;

        Trace.TraceInformation("RunningTimer Deactivated");
    }

    [ReactiveCommand]
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
                    await _mediator.Send(new PlaySound.Command() { SoundType = SoundType.Break });
                    await _notificationService.Show(_languageService.GetString("Success"), _languageService.GetString("FocusCompleted"));
                    _toastService.Display(_languageService.GetString("Success"), _languageService.GetString("FocusCompleted"), NotificationType.Success);

                    var result = await _mediator.Send(new Application.Settings.Get.Query());
                    if (result.IsSuccess && result.Value.IsFocusDescriptionEnabled)
                    {
                        var app = App.Current as App;
                        app?.Open_Click();
                    }
                }
                else if (TimerState == TimerState.Break)
                {
                    await _mediator.Send(new PlaySound.Command() { SoundType = SoundType.Work });
                    await _notificationService.Show(_languageService.GetString("Information"), _languageService.GetString("BreakCompleted"));
                    _toastService.Display(_languageService.GetString("Information"), _languageService.GetString("BreakCompleted"), NotificationType.Information);
                }
                else if (TimerState == TimerState.LongBreak)
                {
                    await _mediator.Send(new PlaySound.Command() { SoundType = SoundType.Work });
                    await _notificationService.Show(_languageService.GetString("Information"), _languageService.GetString("LongBreakCompleted"));
                    _toastService.Display(_languageService.GetString("Information"), _languageService.GetString("LongBreakCompleted"), NotificationType.Information);
                }

                await GoToNextState(isSkipping: false);
            }
        }
        catch(Exception ex)
        {
            Log.Error(ex, "Failed to tick timer {Error}. Going to next state.", ex.Message);

            await GoToNextState(isSkipping: false);

            Restart();
        }
    }

    [ReactiveCommand]
    private async Task EndSession()
    {
        var result = await _mediator.Send(new Application.Settings.Get.Query());
        await HostScreen.Router.NavigateAndReset.Execute(new AdjustTimersViewModel(result.Value, _mediator, _languageService, _toastService, _dialog, _notificationService, HostScreen));
        Trace.TraceInformation("EndSession");
    }

    [ReactiveCommand]
    private async Task SkipToBreak()
    {
        await GoToNextState(isSkipping: true);
        Trace.TraceInformation("SkipToBreak");
    }

    [ReactiveCommand]
    private void StartOrPauseTimer()
    {
        IsRunning = !IsRunning;
    }

    [ReactiveCommand]
    private async Task RestartTimer()
    {
        await StateChanged(TimerState);
    }

    [ReactiveCommand]
    private async Task OpenNotes(Session? session = null)
    {
        try
        { 
            var previousRunningState = IsRunning;
            IsRunning = false;

            var result = await _mediator.Send(new Application.Settings.Get.Query());
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
                            _mediator.Send(new Application.Sessions.Edit.Command() { Session = session }).GetAwaiter().GetResult();
                        }
                    },
                    () => IsRunning = previousRunningState))
                    .TryShow();
            }
        }
        catch(Exception ex)
        {
            Log.Error(ex, "Failed to open notes, {Error}", ex.Message);
        }

        Trace.TraceInformation("OpenNotes");
    }

    [ReactiveCommand]
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

            await _mediator.Send(new Application.Sessions.Create.Command() { Session = session });

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
        var result = await _mediator.Send(new Application.Settings.Get.Query());
        if (!result.IsSuccess)
        {
            Log.Warning("Failed to get settings {Error}", result.Error);
            return new();
        }
        var settings = result.Value;
        return settings;
    }

    [ReactiveCommand]
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

#if DEBUG
        SecondsLeft = 4;
#endif

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