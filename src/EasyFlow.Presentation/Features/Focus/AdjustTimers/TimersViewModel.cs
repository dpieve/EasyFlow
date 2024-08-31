using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyFlow.Application.Settings;
using EasyFlow.Domain.Entities;
using EasyFlow.Presentation.Common;
using EasyFlow.Presentation.Services;
using MediatR;
using ReactiveUI;
using SukiUI.Controls;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace EasyFlow.Presentation.Features.Focus.AdjustTimers;

public sealed partial class TimersViewModel : ViewModelBase
{
    private readonly IMediator _mediator;
    private readonly ILanguageService _languageService;

    [ObservableProperty]
    private int _workMinutes;

    [ObservableProperty]
    private int _breakMinutes;

    [ObservableProperty]
    private int _longBreakMinutes;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SessionsBeforeLongBreakText))]
    private int _sessionsBeforeLongBreak;

    public TimersViewModel(
        IMediator mediator,
        GeneralSettings settings,
        ILanguageService languageService)
    {
        _mediator = mediator;
        _languageService = languageService;

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

    public string SessionsBeforeLongBreakText => $"{SessionsBeforeLongBreak} {ConstantTranslation.Sessions}";

    [RelayCommand]
    private async Task SaveSettings()
    {
        var result = await _mediator.Send(new GetSettingsQuery());
        if (!result.IsSuccess)
        {
            return;
        }

        var settings = result.Value!;
        settings.WorkDurationMinutes = WorkMinutes;
        settings.BreakDurationMinutes = BreakMinutes;
        settings.LongBreakDurationMinutes = LongBreakMinutes;
        settings.WorkSessionsBeforeLongBreak = SessionsBeforeLongBreak;

        _ = await _mediator.Send(new UpdateSettingsCommand() { GeneralSettings = settings });
    }

    public void Adjust(TimerType timerType, AdjustFactor adjust)
    {
        var (success, newValue) = GetNewValue(timerType, adjust);

        if (timerType == TimerType.Work && newValue < BreakMinutes)
        {
            newValue = BreakMinutes;
            SukiHost.ShowToast(_languageService.GetString("Information"), _languageService.GetString("FocusGreaterOrEqualBreak"), SukiUI.Enums.NotificationType.Info);
        }

        if (timerType == TimerType.LongBreak && newValue < BreakMinutes)
        {
            newValue = BreakMinutes;
            SukiHost.ShowToast(_languageService.GetString("Information"), _languageService.GetString("LongBreakGreaterOrEqualBreak"), SukiUI.Enums.NotificationType.Info);
        }

        if (timerType == TimerType.Break && newValue > LongBreakMinutes)
        {
            newValue = LongBreakMinutes;
            SukiHost.ShowToast(_languageService.GetString("Information"), _languageService.GetString("BreakSmallerOrEqualLongBreak"), SukiUI.Enums.NotificationType.Info);
        }

        if (timerType == TimerType.Break && newValue > WorkMinutes)
        {
            newValue = WorkMinutes;
            SukiHost.ShowToast(_languageService.GetString("Information"), _languageService.GetString("BreakSmallerOrEqualFocus"), SukiUI.Enums.NotificationType.Info);
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