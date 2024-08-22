using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyFlow.Common;
using SukiUI.Controls;
using System;

namespace EasyFlow.Features.Focus.RunningTimer;
public sealed partial class EditDescriptionViewModel : ViewModelBase
{
    private readonly Action<string>? _onOk;

    [ObservableProperty]
    private string _description = string.Empty;

    public EditDescriptionViewModel(Action<string>? onOk = null)
    {
        _onOk = onOk;
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
    private static void Cancel() => SukiHost.CloseDialog();
}
