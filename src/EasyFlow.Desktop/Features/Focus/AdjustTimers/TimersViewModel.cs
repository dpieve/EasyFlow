using Avalonia.Controls.Notifications;
using EasyFlow.Desktop.Common;
using EasyFlow.Desktop.Services;
using EasyFlow.Domain.Entities;
using MediatR;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Serilog;
using System.Reactive.Linq;

namespace EasyFlow.Desktop.Features.Focus.AdjustTimers;

public sealed partial class TimersViewModel : ViewModelBase
{
    private readonly IMediator _mediator;
    private readonly ILanguageService _languageService;
    private readonly IToastService _toastService;

    [Reactive]
    private int _workMinutes;

    [Reactive]
    private int _breakMinutes;

    [Reactive]
    private int _longBreakMinutes;

    [Reactive]
    //[NotifyPropertyChangedFor(nameof(SessionsBeforeLongBreakText))]
    private int _sessionsBeforeLongBreak;

    [ObservableAsProperty]
    private string _sessionsBeforeLongBreakText = "0";

    public TimersViewModel(
        IMediator mediator,
        GeneralSettings settings,
        ILanguageService languageService,
        IToastService toastService)
    {
        _mediator = mediator;
        _languageService = languageService;
        _toastService = toastService;

        WorkMinutes = settings.WorkDurationMinutes;
        BreakMinutes = settings.BreakDurationMinutes;
        LongBreakMinutes = settings.LongBreakDurationMinutes;
        SessionsBeforeLongBreak = settings.WorkSessionsBeforeLongBreak;

        this.WhenAnyValue(
            vm => vm.WorkMinutes,
            vm => vm.BreakMinutes,
            vm => vm.LongBreakMinutes,
            vm => vm.SessionsBeforeLongBreak)
            .Skip(1)
            .Throttle(TimeSpan.FromMilliseconds(100))
            .Select(_ => System.Reactive.Unit.Default)
            .InvokeCommand(SaveSettingsCommand);

        _sessionsBeforeLongBreakTextHelper = this.WhenAnyValue(vm => vm.SessionsBeforeLongBreak)
            .Select(sessionsBeforeLongBreak => $"{sessionsBeforeLongBreak} {ConstantTranslation.Sessions}")
            .ToProperty(this, vm => vm.SessionsBeforeLongBreakText);
    }

    [ReactiveCommand]
    private async Task SaveSettings()
    {
        var result = await _mediator.Send(new Application.Settings.Get.Query());
        if (!result.IsSuccess)
        {
            Log.Warning("Failed to get settings: {Error}", result.Error);
            return;
        }

        var settings = result.Value!;
        settings.WorkDurationMinutes = WorkMinutes;
        settings.BreakDurationMinutes = BreakMinutes;
        settings.LongBreakDurationMinutes = LongBreakMinutes;
        settings.WorkSessionsBeforeLongBreak = SessionsBeforeLongBreak;

        _ = await _mediator.Send(new Application.Settings.Edit.Command() { Settings = settings });
    }

    public void Adjust(TimerType timerType, AdjustFactor adjust)
    {
        var (success, newValue) = GetNewValue(timerType, adjust);

        if (timerType == TimerType.Work && newValue < BreakMinutes)
        {
            newValue = BreakMinutes;
            _toastService.Display(_languageService.GetString("Information"), _languageService.GetString("FocusGreaterOrEqualBreak"), NotificationType.Information);
        }

        if (timerType == TimerType.LongBreak && newValue < BreakMinutes)
        {
            newValue = BreakMinutes;
            _toastService.Display(_languageService.GetString("Information"), _languageService.GetString("LongBreakGreaterOrEqualBreak"), NotificationType.Information);
        }

        if (timerType == TimerType.Break && newValue > LongBreakMinutes)
        {
            newValue = LongBreakMinutes;
            _toastService.Display(_languageService.GetString("Information"), _languageService.GetString("BreakSmallerOrEqualLongBreak"), NotificationType.Information);
        }

        if (timerType == TimerType.Break && newValue > WorkMinutes)
        {
            newValue = WorkMinutes;
            _toastService.Display(_languageService.GetString("Information"), _languageService.GetString("BreakSmallerOrEqualFocus"), NotificationType.Information);
        }

        if (success)
        {
            SetNewValue(timerType, newValue);
        }
    }

    private (bool success, int newValue) GetNewValue(TimerType timerType, AdjustFactor adjust)
    {
        var limits = Constants.TimerTypeLimits[timerType];

        var factor = adjust switch
        {
            AdjustFactor.StepForward => 1 * limits.Step,
            AdjustFactor.LongStepForward => 1 * limits.LongStep,
            AdjustFactor.StepBackward => -1 * limits.Step,
            AdjustFactor.LongStepBackward => -1 * limits.LongStep,
            _ => 0
        };

        var baseValue = timerType switch
        {
            TimerType.Work => WorkMinutes,
            TimerType.Break => BreakMinutes,
            TimerType.LongBreak => LongBreakMinutes,
            _ => SessionsBeforeLongBreak
        };

        var newValue = baseValue + factor * limits.Step;

        if (newValue < limits.Min)
        {
            return (success: false, newValue: baseValue);
        }
        if (newValue > limits.Max)
        {
            return (success: false, newValue: baseValue);
        }

        return (success: true, newValue);
    }

    private void SetNewValue(TimerType timerType, int newValue)
    {
        switch (timerType)
        {
            case TimerType.Work:
                WorkMinutes = newValue;
                break;

            case TimerType.Break:
                BreakMinutes = newValue;
                break;

            case TimerType.LongBreak:
                LongBreakMinutes = newValue;
                break;

            default:
                break;
        }
    }
}