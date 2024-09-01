using CommunityToolkit.Mvvm.Input;
using EasyFlow.Application.Settings;
using EasyFlow.Presentation.Common;
using MediatR;
using Serilog;
using SukiUI.Controls;
using System;
using System.Threading.Tasks;

namespace EasyFlow.Presentation.Features.Settings.General;

public sealed partial class DeleteDataViewModel : ViewModelBase
{
    private readonly IMediator _mediator;
    private readonly Action? _onOk;
    private readonly Action? _onCancel;

    public DeleteDataViewModel(
        IMediator mediator,
        Action? onOk = null,
        Action? onCancel = null)
    {
        _mediator = mediator;
        _onOk = onOk;
        _onCancel = onCancel;
    }

    [RelayCommand]
    private async Task Ok()
    {
        var result = await _mediator.Send(new ResetDbQuery());

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

    private static void Close() => SukiHost.CloseDialog();
}