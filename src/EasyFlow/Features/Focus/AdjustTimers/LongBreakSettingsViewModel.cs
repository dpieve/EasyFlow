using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyFlow.Common;
using SukiUI.Controls;
using System;

namespace EasyFlow.Features.Focus.AdjustTimers;

public sealed partial class LongBreakSettingsViewModel : ViewModelBase
{
    private readonly Action<int> _longBreakSessionsResult;

    [ObservableProperty]
    private int _longBreakSessions = 5;

    public LongBreakSettingsViewModel(Action<int> longBreakSessionsResult)
    {
        _longBreakSessionsResult = longBreakSessionsResult;
    }

    [RelayCommand]
    private void OkButton()
    {
        _longBreakSessionsResult(LongBreakSessions);
        CloseButton();
    }

    [RelayCommand]
    private static void CloseButton() => SukiHost.CloseDialog();

}