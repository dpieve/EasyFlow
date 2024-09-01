using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyFlow.Presentation.Common;
using SukiUI.Controls;
using System;

namespace EasyFlow.Presentation.Features.Focus.RunningTimer;

public sealed partial class EditDescriptionViewModel : ViewModelBase
{
    private readonly Action<string>? _onOk;
    private readonly Action? _onCancel;

    [ObservableProperty]
    private string _description = string.Empty;

    public EditDescriptionViewModel(
        string description,
        Action<string>? onOk = null,
        Action? onCancel = null)
    {
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

        SukiHost.CloseDialog();
    }
}