using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyFlow.Application.Services;
using EasyFlow.Application.Sessions;
using EasyFlow.Application.Settings;
using EasyFlow.Domain.Entities;
using EasyFlow.Presentation.Common;
using EasyFlow.Presentation.Features.Focus.AdjustTimers;
using EasyFlow.Presentation.Services;
using Material.Icons;
using MediatR;
using ReactiveUI;
using Serilog;
using SimpleRouter;
using SukiUI.Controls;
using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace EasyFlow.Presentation.Features.Focus.RunningTimer;

public sealed partial class RunningTimerViewModel : ViewModelBase, IRoute, IActivatableRoute
{
    private readonly IMediator _mediator;
    private readonly ILanguageService _languageService;

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
        ILanguageService languageService)
    {
        RouterHost = routerHost;
        _mediator = mediator;
        _languageService = languageService;

        this.WhenAnyValue(vm => vm.SecondsLeft)
            .DistinctUntilChanged()
            .Subscribe(secondsLeft =>
            {
                var minutes = secondsLeft / 60;
                var seconds = secondsLeft % 60;
                TimerText = $"{minutes:D2}:{seconds:D2}";
                ProgressValue = (double)secondsLeft / TotalSeconds * 100;
            });

        this.WhenAnyValue(vm => vm.TimerState)
            .Skip(1)
            .InvokeCommand(StateChangedCommand);

        this.WhenAnyValue(vm => vm.TimerState)
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
            .Subscribe(settings =>
            {
                IsFocusDescriptionVisible = settings.IsFocusDescriptionEnabled;
                TimersBeforeLongBreak = settings.WorkSessionsBeforeLongBreak;
                SelectedTagName = settings.SelectedTag?.Name ?? string.Empty;

                if (IsRunning)
                {
                    return;
                }

                var totalMinutes = settings.WorkDurationMinutes;
                TotalSeconds = totalMinutes * 60;

                var minutes = TotalSeconds / 60;
                var seconds = TotalSeconds % 60;
                TimerText = $"{minutes:D2}:{seconds:D2}";

                SecondsLeft = TotalSeconds;

                // Test
                SecondsLeft = 3;

                IsRunning = true;
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
        if (SecondsLeft > 0)
        {
            SecondsLeft--;
        }
        else if (SecondsLeft == 0)
        {
            if (TimerState == TimerState.Focus)
            {
                await _mediator.Send(new PlaySoundQuery() { SoundType = SoundType.Break });

                await SukiHost.ShowToast(_languageService.GetString("Success"), _languageService.GetString("FocusCompleted"), SukiUI.Enums.NotificationType.Success);
            }
            else if (TimerState == TimerState.Break)
            {
                await _mediator.Send(new PlaySoundQuery() { SoundType = SoundType.Work });

                await SukiHost.ShowToast(_languageService.GetString("Information"), _languageService.GetString("BreakCompleted"), SukiUI.Enums.NotificationType.Info);
            }
            else if (TimerState == TimerState.LongBreak)
            {
                await _mediator.Send(new PlaySoundQuery() { SoundType = SoundType.Work });

                await SukiHost.ShowToast(_languageService.GetString("Information"), _languageService.GetString("LongBreakCompleted"), SukiUI.Enums.NotificationType.Info);
            }

            await GoToNextState(isSkipping: false);
        }
    }

    [RelayCommand]
    private async Task EndSession()
    {
        var result = await _mediator.Send(new GetSettingsQuery());
        RouterHost.Router.NavigateTo(new AdjustTimersViewModel(result.Value, RouterHost, _mediator, _languageService));

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
        var previousRunningState = IsRunning;
        IsRunning = false;

        var result = await _mediator.Send(new GetSettingsQuery());
        var settings = result.Value;

        var isDescriptionEnabled = settings.IsFocusDescriptionEnabled;
        if (isDescriptionEnabled)
        {
            SukiHost.ShowDialog(new EditDescriptionViewModel(Description, (string notes) =>
            {
                Description = notes;
                IsRunning = previousRunningState;

                if (session is not null)
                {
                    session.Description = notes;
                    _mediator.Send(new CreateSessionCommand() { Session = session }).GetAwaiter().GetResult();
                }
            },
            () => IsRunning = previousRunningState),
            allowBackgroundClose: false);
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

        // TEST:
        SecondsLeft = 3;

        IsBreak = state != TimerState.Focus;
    }
}