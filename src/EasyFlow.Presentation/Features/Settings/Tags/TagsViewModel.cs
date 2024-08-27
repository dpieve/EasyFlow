﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using EasyFlow.Application.Tags;
using EasyFlow.Presentation.Common;
using MediatR;
using ReactiveUI;
using SukiUI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace EasyFlow.Presentation.Features.Settings.Tags;

public partial class TagsViewModel : ViewModelBase
{
    private readonly IMediator _mediator;

    [ObservableProperty]
    private int _numTags;

    public TagsViewModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public ObservableCollection<TagItemViewModel> Tags { get; } = [];

    public void Activate()
    {
        Observable
            .StartAsync(GetTags)
            .Where(tags => tags.Count > 0)
            .Select(tags => tags.Select(tag => new TagItemViewModel(tag, _mediator, onDeletedTag: DeletedTag)))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Do(_ => Tags.Clear())
            .Do(tags => Tags.AddRange(tags))
            .Subscribe(_ => NumTags = Tags.Count);
    }

    public void Deactivate()
    {
    }

    [RelayCommand]
    private void AddTag()
    {
        SukiHost.ShowDialog(new AddTagViewModel(_mediator, onOk: AddedTag), allowBackgroundClose: true);
    }

    private void AddedTag(Domain.Entities.Tag tag)
    {
        var newItem = new TagItemViewModel(tag, _mediator, onDeletedTag: DeletedTag);
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

        var removed = Tags.Remove(deletedItem);
        if (!removed)
        {
            SukiHost.ShowToast("Failed to remove", "Please reload to remove the tag.", SukiUI.Enums.NotificationType.Error);
        }

        NumTags = Tags.Count;
    }

    private async Task<List<Domain.Entities.Tag>> GetTags()
    {
        var result = await _mediator.Send(new GetTagsQuery());

        if (result.IsSuccess)
        {
            return result.Value;
        }

        await SukiHost.ShowToast("Failed to load", "Tags couldn't be loaded.", SukiUI.Enums.NotificationType.Error);
        return [];
    }
}