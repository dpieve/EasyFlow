using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyFlow.Application.Tags;
using EasyFlow.Domain.Entities;
using EasyFlow.Presentation.Common;
using MediatR;
using SukiUI.Controls;
using System;
using System.Threading.Tasks;

namespace EasyFlow.Presentation.Features.Settings.Tags;

public sealed partial class AddTagViewModel : ViewModelBase
{
    private readonly IMediator _mediator;
    private readonly Action<Tag> _onOk;
    private readonly Tag _tag;

    [ObservableProperty]
    private string _tagName;

    public AddTagViewModel(
        IMediator mediator,
        Action<Tag> onOk,
        Tag? initialTag = null)
    {
        _mediator = mediator;
        _onOk = onOk;
        _tag = initialTag ?? new() { };

        TagName = _tag.Name;
    }

    [RelayCommand]
    private async Task Ok()
    {
        _tag.Name = TagName;

        var command = new CreateTagCommand
        {
            Tag = _tag
        };

        var result = await _mediator.Send(command);
        if (result.IsSuccess)
        {
            _onOk(result.Value!);
        }
        
        Cancel();
    }

    [RelayCommand]
    private static void Cancel() => SukiHost.CloseDialog();
}