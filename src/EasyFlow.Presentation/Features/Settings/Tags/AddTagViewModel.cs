using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyFlow.Application.Tags;
using EasyFlow.Domain.Entities;
using EasyFlow.Presentation.Common;
using EasyFlow.Presentation.Services;
using MediatR;
using SukiUI.Controls;
using System;
using System.Threading.Tasks;

namespace EasyFlow.Presentation.Features.Settings.Tags;

public sealed partial class AddTagViewModel : ViewModelBase
{
    private readonly IMediator _mediator;
    private readonly Action<Tag> _onOk;
    private readonly ILanguageService _languageService;
    private readonly Action? _onCancel;
    private readonly Tag _tag;

    [ObservableProperty]
    private string _tagName;

    public AddTagViewModel(
        IMediator mediator,
        ILanguageService languageService,
        Action<Tag> onOk,
        Action? onCancel = null,
        Tag? initialTag = null)
    {
        _mediator = mediator;
        _onOk = onOk;
        _languageService = languageService;
        _onCancel = onCancel;
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
        else
        {
            var code = result.Error.Code;
            var error = _languageService.GetString(code);

            if (error == ConstantTranslation.CanNotMoreThanMax)
            {
                error += $" {Tag.MaxNumTags}";
            }    

            await SukiHost.ShowToast(_languageService.GetString("Failure"), error);
        }

        Cancel();
    }

    [RelayCommand]
    private void Cancel()
    {
        _onCancel?.Invoke();
        SukiHost.CloseDialog();
    }
}