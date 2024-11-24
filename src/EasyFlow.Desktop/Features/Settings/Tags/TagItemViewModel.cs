using EasyFlow.Desktop.Common;
using EasyFlow.Desktop.Services;
using EasyFlow.Domain.Entities;
using MediatR;
using ReactiveUI.SourceGenerators;
using SukiUI.Dialogs;
using System;
using System.Threading.Tasks;

namespace EasyFlow.Desktop.Features.Settings.Tags;

public sealed partial class TagItemViewModel : ViewModelBase
{
    private readonly IMediator _mediator;
    private readonly Action<Tag> _onDeletedTag;
    private readonly ILanguageService _languageService;
    private readonly IToastService _toastService;
    private readonly ISukiDialogManager _dialog;

    [Reactive]
    private string _name;

    public TagItemViewModel(Tag tag,
                            IMediator mediator,
                            Action<Tag> onDeletedTag,
                            ILanguageService languageService,
                            IToastService toastService,
                            ISukiDialogManager dialog)
    {
        Tag = tag;
        _mediator = mediator;
        _onDeletedTag = onDeletedTag;
        _languageService = languageService;
        _toastService = toastService;
        _dialog = dialog;
        
        Name = tag.Name;
    }

    public Tag Tag { get; }

    [ReactiveCommand]
    private void EditTag()
    {
        _dialog.CreateDialog()
            .WithViewModel(dialog => new AddTagViewModel(dialog, _mediator, _languageService, _toastService, onOk: EditedTag, initialTag: Tag))
            .TryShow();
    }

    [ReactiveCommand]
    private async Task DeleteTag()
    {
        var result = await _mediator.Send(new Application.Tags.Delete.Command
        {
            Tag = Tag
        });

        if (!result.IsSuccess)
        {
            _toastService.Display(_languageService.GetString("Information"), _languageService.GetString(result.Error.Code), Avalonia.Controls.Notifications.NotificationType.Information);
            return;
        }

        _onDeletedTag(Tag);
    }

    private void EditedTag(Tag tag)
    {
        Tag.Name = tag.Name;
        Name = Tag.Name;
    }
}