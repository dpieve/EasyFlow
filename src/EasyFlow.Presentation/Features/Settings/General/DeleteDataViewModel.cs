using CommunityToolkit.Mvvm.Input;
using EasyFlow.Application.Settings;
using EasyFlow.Presentation.Common;
using MediatR;
using SukiUI.Controls;
using System;
using System.Threading.Tasks;

namespace EasyFlow.Presentation.Features.Settings.General;
public sealed partial class DeleteDataViewModel : ViewModelBase
{
    private readonly IMediator _mediator;
    private readonly Action? _onOk;

    public DeleteDataViewModel(
        IMediator mediator,
        Action? onOk = null)
    {
        _mediator = mediator;
        _onOk = onOk;
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
        Close();
    }

    private static void Close() => SukiHost.CloseDialog();
}
