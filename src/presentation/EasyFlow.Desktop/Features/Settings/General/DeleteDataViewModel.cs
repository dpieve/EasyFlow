using CommunityToolkit.Mvvm.Input;
using EasyFlow.Application.Services;
using EasyFlow.Desktop.Common;
using MediatR;
using SukiUI.Dialogs;
using System;
using System.Threading.Tasks;

namespace EasyFlow.Desktop.Features.Settings.General;

public sealed partial class DeleteDataViewModel : ViewModelBase
{
    private readonly ISukiDialog _dialog;
    private readonly IMediator _mediator;
    private readonly Action? _onOk;
    private readonly Action? _onCancel;

    public DeleteDataViewModel(
        ISukiDialog dialog,
        IMediator mediator,
        Action? onOk = null,
        Action? onCancel = null)
    {
        _dialog = dialog;
        _mediator = mediator;
        _onOk = onOk;
        _onCancel = onCancel;
    }

    [RelayCommand]
    private async Task Ok()
    {
        var result = await _mediator.Send(new RestartDatabase.Command());

        Close();

        if (result.IsSuccess && _onOk is not null)
        {
            _onOk();
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        _onCancel?.Invoke();
        Close();
    }

    private void Close() 
    {
        _dialog.Dismiss();
    } 
}