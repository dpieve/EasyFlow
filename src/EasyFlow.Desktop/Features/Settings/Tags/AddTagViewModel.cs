using EasyFlow.Desktop.Common;
using EasyFlow.Desktop.Services;
using EasyFlow.Domain.Entities;
using MediatR;
using ReactiveUI.SourceGenerators;
using SukiUI.Dialogs;

namespace EasyFlow.Desktop.Features.Settings.Tags;

public sealed partial class AddTagViewModel : ViewModelBase
{
    private readonly ISukiDialog _dialog;
    private readonly IMediator _mediator;
    private readonly Action<Tag> _onOk;
    private readonly ILanguageService _languageService;
    private readonly IToastService _toastService;
    private readonly Action? _onCancel;
    private readonly Tag _tag;

    private bool _isEditing;

    [Reactive]
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

        _isEditing = initialTag is not null;

        TagName = _tag.Name;
    }

    [ReactiveCommand]
    private async Task Ok()
    {
        _tag.Name = TagName;

        var result = _isEditing ?
                        await _mediator.Send(new Application.Tags.Edit.Command { Tag = _tag }) :
                        await _mediator.Send(new Application.Tags.Create.Command { Tag = _tag });

        if (result.IsSuccess)
        {
            _onOk(_tag);
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

    [ReactiveCommand]
    private void Cancel()
    {
        _onCancel?.Invoke();
        _dialog.Dismiss();
    }
}