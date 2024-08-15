using CommunityToolkit.Mvvm.Input;
using EasyFlow.Common;
using SukiUI.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EasyFlow.Services;
using EasyFlow.Data;
using System.Diagnostics;

namespace EasyFlow.Features.Settings.Tags;

public partial class TagsViewModel : ViewModelBase
{
    private readonly ITagService _tagService;

    public TagsViewModel(ITagService tagService)
    {
        _tagService = tagService;
    }

    public ObservableCollection<TagItemViewModel> Tags { get; } = [];

    public void Activate()
    {
        var tags = _tagService.GetAll();

        if (tags.Error is not null)
        {
            SukiHost.ShowToast("Failed to load tags", tags.Error.Message!, SukiUI.Enums.NotificationType.Error);
            return;
        }

        Reload(tags.Value!);

        Debug.WriteLine("Activated TagsViewModel");
    }
    
    public void Deactivate()
    {
        Debug.WriteLine("Deactivated TagsViewModel");
    }

    [RelayCommand]
    private void AddTag()
    {
        SukiHost.ShowDialog(new AddTagViewModel(_tagService, onOk: OnOkAddTag), allowBackgroundClose: false);
    }

    private void OnOkAddTag(Tag tag)
    {
        Tags.Add(new TagItemViewModel(tag, _tagService, onDeletedTag: OnDeletedTag));
    }

    public void OnDeletedTag(Tag tag)
    {
        var tagItem = Tags.FirstOrDefault(x => x.Tag.Id == tag.Id);
        if (tagItem is not null)
        {
            Tags.Remove(tagItem);
        }
    }

    private void Reload(List<Tag> tags)
    {
        Tags.Clear();
        foreach (var tag in tags)
        {
            Tags.Add(new TagItemViewModel(tag, _tagService, onDeletedTag: OnDeletedTag));
        }
    }
}
