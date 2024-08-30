using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using EasyFlow.Application.Tags;
using EasyFlow.Presentation.Common;
using EasyFlow.Presentation.Services;
using MediatR;
using ReactiveUI;
using SukiUI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace EasyFlow.Presentation.Features.Settings.Tags;

public partial class TagsViewModel : ViewModelBase
{
    private readonly IMediator _mediator;
    private readonly ILanguageService _languageService;

    [ObservableProperty]
    private int _numTags;

    public TagsViewModel(IMediator mediator, ILanguageService languageService)
    {
        _mediator = mediator;
        _languageService = languageService;
    }

    public ObservableCollection<TagItemViewModel> Tags { get; } = [];

    public void Activate()
    {
        Observable
            .StartAsync(GetTags)
            .Where(tags => tags.Count > 0)
            .Select(tags => tags.Select(tag => new TagItemViewModel(tag, _mediator, onDeletedTag: DeletedTag, _languageService)))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Do(_ => Tags.Clear())
            .Do(tags => Tags.AddRange(tags))
            .Subscribe(_ => NumTags = Tags.Count);
    }

    public void Deactivate()
    {
        Debug.WriteLine("Deactivating TagsViewModel");
    }

    [RelayCommand]
    private void AddTag()
    {
        SukiHost.ShowDialog(new AddTagViewModel(_mediator, onOk: AddedTag), allowBackgroundClose: true);
    }

    private void AddedTag(Domain.Entities.Tag tag)
    {
        var newItem = new TagItemViewModel(tag, _mediator, onDeletedTag: DeletedTag, _languageService);
        Tags.Add(newItem);
        NumTags = Tags.Count;
    }

    private void DeletedTag(Domain.Entities.Tag tag)
    {
        var deletedItem = Tags.FirstOrDefault(x => x.Tag.Id == tag.Id);

        if (deletedItem is null)
        {
            return;
        }

        _ = Tags.Remove(deletedItem);
        NumTags = Tags.Count;
    }

    private async Task<List<Domain.Entities.Tag>> GetTags()
    {
        var result = await _mediator.Send(new GetTagsQuery());

        if (result.IsSuccess)
        {
            return result.Value;
        }

        return [];
    }
}