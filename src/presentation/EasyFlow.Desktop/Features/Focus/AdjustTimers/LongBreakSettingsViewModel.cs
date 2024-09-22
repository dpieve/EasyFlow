using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyFlow.Desktop.Common;
using SukiUI.Dialogs;
using System;

namespace EasyFlow.Desktop.Features.Focus.AdjustTimers;

public sealed partial class LongBreakSettingsViewModel : ViewModelBase
{
    private readonly ISukiDialog _dialog;
    private readonly Action<int> _onOk;

    [ObservableProperty]
    private int _longBreakSessions;

    public LongBreakSettingsViewModel(ISukiDialog dialog, int longBreakSessions, Action<int> onOk)
    {
        _dialog = dialog;
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
    private void CloseButton() => _dialog.Dismiss();
}