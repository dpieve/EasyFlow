using CommunityToolkit.Mvvm.ComponentModel;
using EasyFlow.Application.Tags;
using EasyFlow.Domain.Entities;
using EasyFlow.Presentation.Common;
using MediatR;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System;
using ReactiveUI;
using CommunityToolkit.Mvvm.Input;
using EasyFlow.Presentation.Services;
using SukiUI.Controls;
using System.Linq;
using DynamicData;
using System.Diagnostics;

namespace EasyFlow.Presentation.Features.Dashboard.DisplayControls;

public sealed partial class DisplayControlsViewModel : ViewModelBase
{
    private readonly IMediator _mediator;
    private readonly ILanguageService _languageService;

    [ObservableProperty]
    private bool _isGeneratingReport = false;

    [ObservableProperty]
    private FilterPeriod _selectedFilterPeriod = FilterPeriod.Days7;

    [ObservableProperty]
    private Tag? _selectedTag;

    [ObservableProperty]
    private SessionType  _selectedSessionType = SessionType.Focus;

    [ObservableProperty]
    private DisplayType _selectedDisplayType = DisplayType.BarChart;

    public DisplayControlsViewModel(IMediator mediator, ILanguageService languageService)
    {
        _mediator = mediator;
        _languageService = languageService;

        SessionTypes.AddRange(Enum.GetValues(typeof(SessionType)).Cast<SessionType>());
        DisplayTypes.AddRange(Enum.GetValues(typeof(DisplayType)).Cast<DisplayType>());
        FilterPeriods.AddRange(FilterPeriod.Filters);
    }

    public ObservableCollection<Tag> Tags { get; } = [];
    public ObservableCollection<SessionType> SessionTypes { get; } = [];
    public ObservableCollection<FilterPeriod> FilterPeriods { get; } = [];
    public ObservableCollection<DisplayType> DisplayTypes { get; } = [];

    public IObservable<Display> ChangedControls => this.WhenAnyValue(
        x => x.SelectedFilterPeriod,
        x => x.SelectedTag,
        x => x.SelectedSessionType,
        x => x.SelectedDisplayType)
        .Skip(1)
        .Where(x => x.Item1 is not null && x.Item2 is not null)
        .Select(_ => GetDisplayControls());

    public void Activated()
    {
        Observable
           .StartAsync(GetTags)
           .WhereNotNull()
           .Subscribe(tags =>
           {
               Tags.Clear();
               foreach (var tag in tags)
               {
                   Tags.Add(tag);
               }

               SelectedTag = Tags[0];

               Debug.WriteLine("Updated tags - DisplayControls");
           });
    }

    public void Deactivated()
    {
    }

    public Display GetDisplayControls() => 
        new Display(SelectedFilterPeriod, SelectedTag!, SelectedSessionType, SelectedDisplayType);

    private async Task<List<Tag>> GetTags()
    {
        var result = await _mediator.Send(new GetTagsQuery());
        return result.Value;
    }

    [RelayCommand]
    private async Task FullReport()
    {
        IsGeneratingReport = true;
        
        var result = await GenerateReportHandler.Handle(_mediator);
        if (result.IsSuccess)
        {
            await SukiHost.ShowToast(_languageService.GetString("Success"), _languageService.GetString("SuccessGeneratedReport"), SukiUI.Enums.NotificationType.Success);
        }

        IsGeneratingReport = false;
    }
}
