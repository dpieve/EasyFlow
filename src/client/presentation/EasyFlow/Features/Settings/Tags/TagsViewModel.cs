using EasyFlow.Common;
using EasyFlow.Domain.Entities;
using EasyFlow.Domain.Services;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading.Tasks;

namespace EasyFlow.Features.Settings.Tags;

public sealed partial class TagsViewModel : ViewModelBase
{
    [Reactive] private string _name;
    [Reactive] private string _errorMessage;

    private readonly ITagService _tagService;

    public TagsViewModel(ITagService tagService)
    {
        _tagService = tagService;

        Name = string.Empty;
        ErrorMessage = string.Empty;

        RxApp.MainThreadScheduler.Schedule(LoadTags);
    }

    public ObservableCollection<TagItemViewModel> Tags { get; } = [];
    public ViewModelActivator Activator { get; } = new();

    [ReactiveCommand]
    private void OnBack()
    {
        ErrorMessage = string.Empty;
    }

    [ReactiveCommand]
    private async Task OnAdd()
    {
        if (string.IsNullOrEmpty(Name))
        {
            ErrorMessage = "Tag name can't be empty.";
            return;
        }

        if (string.IsNullOrWhiteSpace(Name))
        {
            ErrorMessage = "Tag name can't be empty.";
            return;
        }

        if (Tags.Count >= Tag.MaxNumTags)
        {
            ErrorMessage = "You can't add more than 10 tags.";
            return;
        }

        var existingTag = Tags.FirstOrDefault(t => t.Name == Name);
        if (existingTag is not null)
        {
            ErrorMessage = "Tag already exists.";
            return;
        }

        var addedTag = await _tagService.AddTagAsync(Tag.CreateTag(Name));
        Tags.Add(new TagItemViewModel(addedTag, DeleteItem, _tagService));

        Name = string.Empty;
        ErrorMessage = string.Empty;
    }

    private void DeleteItem(TagItemViewModel tagItemViewModel)
    {
        if (Tags.Count <= Tag.MinNumTags)
        {
            ErrorMessage = "You can't delete the last tag.";
            return;
        }

        _tagService.DeleteTagAsync(tagItemViewModel.Tag);

        Tags.Remove(tagItemViewModel);
        ErrorMessage = string.Empty;
    }

    private async void LoadTags()
    {
        var tags = await _tagService.GetTagsAsync();
        foreach (var tag in tags)
        {
            Tags.Add(new TagItemViewModel(tag, DeleteItem, _tagService));
        }
    }
}