using EasyFocus.Common;
using EasyFocus.Domain.Entities;
using EasyFocus.Domain.Services;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace EasyFocus.Features.Settings.Tags;

public sealed partial class TagItemViewModel : ViewModelBase
{
    private readonly Action<TagItemViewModel> _deleteAction;
    private readonly ITagService? _tagService;

    [Reactive] private string _name;
    [Reactive] private bool _isEditing;
    [Reactive] private string _typingName = string.Empty;

    public TagItemViewModel(Tag tag, Action<TagItemViewModel> deleteAction, ITagService? tagService)
    {
        Name = tag.Name;
        Tag = tag;
        _deleteAction = deleteAction;
        _tagService = tagService;

        this.WhenAnyValue(vm => vm.IsEditing)
            .Skip(1)
            .Where(e => !e)
            .Select(_ => Unit.Default)
            .InvokeCommand(OnSaveCommand);
    }

    public Tag Tag { get; set; }

    [ReactiveCommand]
    private void OnDelete()
    {
        _deleteAction(this);
    }

    [ReactiveCommand]
    private void OnEdit()
    {
        TypingName = Name;
        IsEditing = true;
    }

    [ReactiveCommand]
    private async Task OnSave()
    {
        TypingName = TypingName.Trim();

        if (string.IsNullOrWhiteSpace(TypingName) || string.IsNullOrEmpty(TypingName))
        {
            return;
        }

        Name = TypingName;
        IsEditing = false;

        if (Tag.Id != 0 && _tagService is not null)
        {
            var tag = Tag.CreateTag(Name);
            tag.Id = Tag.Id;
            var result = await _tagService.UpdateTagAsync(tag);
            if (result == true)
            {
                Tag = tag;
            }
        }
    }
}