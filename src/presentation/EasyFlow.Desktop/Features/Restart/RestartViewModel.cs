﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyFlow.Desktop.Common;
using ReactiveUI;
using SukiUI.Dialogs;
using System;
using System.Reactive.Linq;

namespace EasyFlow.Desktop.Features.Restart;

public sealed partial class RestartViewModel : ViewModelBase
{
    private readonly ISukiDialog _dialog;
    private Action _onOk;

    [ObservableProperty]
    private bool _isRestarting = true;

    [ObservableProperty]
    private int _secondsLeft;

    public RestartViewModel(ISukiDialog dialog, Action onOk, int secondsBeforeRestart)
    {
        _dialog = dialog;
        _onOk = onOk;

        if (secondsBeforeRestart <= 0)
        {
            throw new ArgumentException("Seconds before restart must be greater than 0", nameof(secondsBeforeRestart));
        }

        SecondsLeft = secondsBeforeRestart;

        Observable
            .Interval(TimeSpan.FromSeconds(1))
            .Where(_ => IsRestarting)
            .Select(_ => System.Reactive.Unit.Default)
            .ObserveOn(RxApp.MainThreadScheduler)
            .InvokeCommand(TimerTickCommand);

        this.WhenAnyValue(vm => vm.SecondsLeft)
            .Where(s => s <= 0)
            .Subscribe(_ =>
            {
                Ok();
            });
    }

    [RelayCommand]
    private void Ok()
    {
        IsRestarting = false;

        _onOk();

        //SukiHost.CloseDialog();
    }

    [RelayCommand]
    private void TimerTick()
    {
        if (SecondsLeft == 0)
        {
            return;
        }

        --SecondsLeft;
    }
}