using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using EasyFlow.Application.Tags;
using EasyFlow.Presentation.Common;
using EasyFlow.Presentation.Services;
using MediatR;
using ReactiveUI;
using SukiUI.Dialogs;
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
    private readonly IToastService _toastService;
    private readonly ISukiDialogManager _dialog;
    [ObservableProperty]
    private int _numTags;

    [ObservableProperty]
    private bool _isAddBusy;

    public TagsViewModel(
        IMediator mediator, 
        ILanguageService languageService,
        IToastService toastService,
        ISukiDialogManager dialog)
    {
        _mediator = mediator;
        _languageService = languageService;
        _toastService = toastService;
        _dialog = dialog;
    }

    public ObservableCollection<TagItemViewModel> Tags { get; } = [];

    public void Activate()
    {
        Observable
            .StartAsync(GetTags)
            .Where(tags => tags.Count > 0)
            .Select(tags => tags.Select(tag => new TagItemViewModel(tag, _mediator, onDeletedTag: DeletedTag, _languageService, toastService: _toastService, _dialog)))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Do(_ => Tags.Clear())
            .Do(tags => Tags.AddRange(tags))
            .Subscribe(_ => NumTags = Tags.Count);
    }

    public void Deactivate()
    {
        Trace.TraceInformation("Deactivating TagsViewModel");
    }

    [RelayCommand]
    private void AddTag()
    {
        IsAddBusy = true;
        _dialog.CreateDialog()
            .WithViewModel(dialog => (new AddTagViewModel(dialog, _mediator, _languageService, _toastService, onOk: AddedTag, onCancel: () => IsAddBusy = false), allowBackgroundClose: false))
            .TryShow();
    }

    private void AddedTag(Domain.Entities.Tag tag)
    {
        var newItem = new TagItemViewModel(tag, _mediator, onDeletedTag: DeletedTag, _languageService, toastService: _toastService, _dialog);
        Tags.Add(newItem);
        NumTags = Tags.Count;

        IsAddBusy = false;
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