using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyFlow.Desktop.Common;
using SukiUI.Dialogs;
using System;

namespace EasyFlow.Desktop.Features.Focus.RunningTimer;

public sealed partial class EditDescriptionViewModel : ViewModelBase
{
    private readonly ISukiDialog _dialog;
    private readonly Action<string>? _onOk;
    private readonly Action? _onCancel;

    [ObservableProperty]
    private string _description = string.Empty;

    public EditDescriptionViewModel(
        ISukiDialog dialog,
        string description,
        Action<string>? onOk = null,
        Action? onCancel = null)
    {
        _dialog = dialog;
        Description = description;

        _onOk = onOk;
        _onCancel = onCancel;
    }

    [RelayCommand]
    private void Ok()
    {
        if (_onOk is not null)
        {
            _onOk(Description);
        }

        Cancel();
    }

    [RelayCommand]
    private void Cancel()
    {
        if (_onCancel is not null)
        {
            _onCancel();
        }

        _dialog.Dismiss();
    }
}