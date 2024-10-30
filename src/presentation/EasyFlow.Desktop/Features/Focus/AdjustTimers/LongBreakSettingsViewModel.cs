using EasyFlow.Desktop.Common;
using ReactiveUI.SourceGenerators;
using SukiUI.Dialogs;
using System;

namespace EasyFlow.Desktop.Features.Focus.AdjustTimers;

public sealed partial class LongBreakSettingsViewModel : ViewModelBase
{
    private readonly ISukiDialog _dialog;
    private readonly Action<int> _onOk;

    [Reactive]
    private int _longBreakSessions;

    public LongBreakSettingsViewModel(ISukiDialog dialog, int longBreakSessions, Action<int> onOk)
    {
        _dialog = dialog;
        LongBreakSessions = longBreakSessions;
        _onOk = onOk;
    }

    [ReactiveCommand]
    private void OkButton()
    {
        _onOk(LongBreakSessions);
        CloseButton();
    }

    [ReactiveCommand]
    private void CloseButton() => _dialog.Dismiss();
}