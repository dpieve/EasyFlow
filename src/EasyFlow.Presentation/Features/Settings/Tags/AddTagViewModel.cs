using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyFlow.Application.Tags;
using EasyFlow.Domain.Entities;
using EasyFlow.Presentation.Common;
using EasyFlow.Presentation.Services;
using MediatR;
using SukiUI.Dialogs;
using System;
using System.Threading.Tasks;

namespace EasyFlow.Presentation.Features.Settings.Tags;

public sealed partial class AddTagViewModel : ViewModelBase
{
    private readonly ISukiDialog _dialog;
    private readonly IMediator _mediator;
    private readonly Action<Tag> _onOk;
    private readonly ILanguageService _languageService;
    private readonly IToastService _toastService;
    private readonly Action? _onCancel;
    private readonly Tag _tag;

    [ObservableProperty]
    private string _tagName;

    public AddTagViewModel(
        ISukiDialog dialog,
        IMediator mediator,
        ILanguageService languageService,
        IToastService toastService,
        Action<Tag> onOk,
        Action? onCancel = null,
        Tag? initialTag = null)
    {
        _dialog = dialog;
        _mediator = mediator;
        _onOk = onOk;
        _languageService = languageService;
        _toastService = toastService;
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

            _toastService.Display(_languageService.GetString("Failure"), error, Avalonia.Controls.Notifications.NotificationType.Information);
        }

        Cancel();
    }

    [RelayCommand]
    private void Cancel()
    {
        _onCancel?.Invoke();
        _dialog.Dismiss();
    }
}