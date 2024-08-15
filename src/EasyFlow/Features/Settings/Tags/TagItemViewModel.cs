using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyFlow.Common;
using EasyFlow.Data;
using EasyFlow.Services;
using ReactiveUI;
using SukiUI.Controls;
using System;
using System.Threading.Tasks;

namespace EasyFlow.Features.Settings.Tags;

public sealed partial class TagItemViewModel : ViewModelBase
{
    private readonly ITagService _tagService;
    private readonly Action<Tag> _onDeletedTag;

    [ObservableProperty]
    private string _name;

    public TagItemViewModel(Tag tag, ITagService tagService, Action<Tag> onDeletedTag)
    {
        Tag = tag;
        _tagService = tagService;
        _onDeletedTag = onDeletedTag;

        Name = tag.Name;
    }

    public Tag Tag { get; }

    [RelayCommand]
    private void EditTag()
    {
        SukiHost.ShowDialog(new AddTagViewModel(_tagService, OnOkEditTag, Tag), allowBackgroundClose: false);
    }

    private void OnOkEditTag(Tag tag)
    {
        Tag.Name = tag.Name;
        Name = Tag.Name;
    }

    [RelayCommand]
    private async Task DeleteTag()
    {
        var result = await _tagService.DeleteAsync(Tag);
        if (result.Error is not null)
        {
            await SukiHost.ShowToast("Failed to delete tag", result.Error.Message!, SukiUI.Enums.NotificationType.Error);
            return;
        }

        _onDeletedTag(Tag);
    }
}