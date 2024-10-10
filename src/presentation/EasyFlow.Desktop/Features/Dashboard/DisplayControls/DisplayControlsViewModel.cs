using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using EasyFlow.Application.Tags;
using EasyFlow.Desktop.Common;
using EasyFlow.Desktop.Services;
using EasyFlow.Domain.Entities;
using MediatR;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace EasyFlow.Desktop.Features.Dashboard.DisplayControls;

public sealed partial class DisplayControlsViewModel : ViewModelBase
{
    private readonly IMediator _mediator;
    private readonly ILanguageService _languageService;
    private readonly IToastService _toastService;

    [ObservableProperty]
    private bool _isGeneratingReport = false;

    [ObservableProperty]
    private FilterPeriod _selectedFilterPeriod = FilterPeriod.Days7;

    [ObservableProperty]
    private Tag? _selectedTag;

    [ObservableProperty]
    private SessionType _selectedSessionType = SessionType.Focus;

    [ObservableProperty]
    private DisplayType _selectedDisplayType = DisplayType.BarChart;

    public DisplayControlsViewModel(IMediator mediator, ILanguageService languageService, IToastService toastService)
    {
        _mediator = mediator;
        _languageService = languageService;
        _toastService = toastService;

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
           });
    }

    public void Deactivated()
    {
        Trace.TraceInformation("Deactivated DisplayControlsViewModel");
    }

    public Display GetDisplayControls() =>
        new(SelectedFilterPeriod, SelectedTag!, SelectedSessionType, SelectedDisplayType);

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
            _toastService.Display(_languageService.GetString("Success"), _languageService.GetString("SuccessGeneratedReport"), NotificationType.Success);
        }

        IsGeneratingReport = false;
    }
}