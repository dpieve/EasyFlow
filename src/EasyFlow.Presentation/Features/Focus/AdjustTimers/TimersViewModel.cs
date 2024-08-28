using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyFlow.Application.Settings;
using EasyFlow.Domain.Entities;
using EasyFlow.Presentation.Common;
using MediatR;
using ReactiveUI;
using SukiUI.Controls;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace EasyFlow.Presentation.Features.Focus.AdjustTimers;

public sealed partial class TimersViewModel : ViewModelBase
{
    private readonly IMediator _mediator;

    [ObservableProperty]
    private int _workMinutes;

    [ObservableProperty]
    private int _breakMinutes;

    [ObservableProperty]
    private int _longBreakMinutes;

    [ObservableProperty]
    private int _sessionsBeforeLongBreak;

    public TimersViewModel(IMediator mediator, GeneralSettings settings)
    {
        _mediator = mediator;

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
    }

    [RelayCommand]
    private async Task SaveSettings()
    {
        var result = await _mediator.Send(new GetSettingsQuery());
        if (!result.IsSuccess)
        {
            await SukiHost.ShowToast("Failed to save", "Failed to save the adjusted settings");
            return;
        }

        var settings = result.Value!;
        settings.WorkDurationMinutes = WorkMinutes;
        settings.BreakDurationMinutes = BreakMinutes;
        settings.LongBreakDurationMinutes = LongBreakMinutes;
        settings.WorkSessionsBeforeLongBreak = SessionsBeforeLongBreak;

        var resultUpdate = await _mediator.Send(new UpdateSettingsCommand() { GeneralSettings = settings });
        if (!resultUpdate.IsSuccess)
        {
            await SukiHost.ShowToast("Failed to update", "Failed to update the settings");
        }
    }

    public void Adjust(TimerType timerType, AdjustFactor adjust)
    {
        var (success, newValue) = GetNewValue(timerType, adjust);

        if (timerType == TimerType.Work && newValue < BreakMinutes)
        {
            newValue = BreakMinutes;
            SukiHost.ShowToast("Adjust timer", "Focus time must be greater or equal break time", SukiUI.Enums.NotificationType.Warning);
        }

        if (timerType == TimerType.LongBreak && newValue < BreakMinutes)
        {
            newValue = BreakMinutes;
            SukiHost.ShowToast("Adjust timer", "Long break time must be greater or equal Break time", SukiUI.Enums.NotificationType.Warning);
        }

        if (timerType == TimerType.Break && newValue > LongBreakMinutes)
        {
            newValue = LongBreakMinutes;
            SukiHost.ShowToast("Adjust timer", "Break time must be smaller or equal Long break time", SukiUI.Enums.NotificationType.Warning);
        }

        if (timerType == TimerType.Break && newValue > WorkMinutes)
        {
            newValue = WorkMinutes;
            SukiHost.ShowToast("Adjust timer", "Break time must be smaller or equal Focus time", SukiUI.Enums.NotificationType.Warning);
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