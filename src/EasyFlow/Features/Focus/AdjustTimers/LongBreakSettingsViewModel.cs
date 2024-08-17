using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyFlow.Common;
using SukiUI.Controls;
using System;

namespace EasyFlow.Features.Focus.AdjustTimers;

public sealed partial class LongBreakSettingsViewModel : ViewModelBase
{
    private readonly Action<int> _onOk;

    [ObservableProperty]
    private int _longBreakSessions = 5;

    public LongBreakSettingsViewModel(int longBreakSessions, Action<int> onOk)
    {
        LongBreakSessions = longBreakSessions;
        _onOk = onOk;
    }

    [RelayCommand]
    private void OkButton()
    {
        _onOk(LongBreakSessions);
        CloseButton();
    }

    [RelayCommand]
    private static void CloseButton() => SukiHost.CloseDialog();

}