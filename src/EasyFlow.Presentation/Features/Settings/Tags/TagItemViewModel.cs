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

public sealed partial class TagItemViewModel : ViewModelBase
{
    private readonly IMediator _mediator;
    private readonly Action<Tag> _onDeletedTag;
    private readonly ILanguageService _languageService;

    [ObservableProperty]
    private string _name;

    public TagItemViewModel(Tag tag,
                            IMediator mediator,
                            Action<Tag> onDeletedTag,
                            ILanguageService languageService)
    {
        Tag = tag;
        _mediator = mediator;
        _onDeletedTag = onDeletedTag;
        _languageService = languageService;

        Name = tag.Name;
    }

    public Tag Tag { get; }

    [RelayCommand]
    private void EditTag()
    {
        //SukiHost.ShowDialog(new AddTagViewModel(_mediator, _languageService, onOk: EditedTag, initialTag: Tag), allowBackgroundClose: false);
    }

    [RelayCommand]
    private async Task DeleteTag()
    {
        var command = new DeleteTagCommand
        {
            Tag = Tag
        };

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            //await SukiHost.ShowToast(_languageService.GetString("Information"), _languageService.GetString(result.Error.Code));
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