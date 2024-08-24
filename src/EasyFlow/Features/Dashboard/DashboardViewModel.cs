using CommunityToolkit.Mvvm.ComponentModel;
using EasyFlow.Common;
using EasyFlow.Data;
using EasyFlow.Services;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using SukiUI.Controls;
using System.Collections.ObjectModel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using ReactiveUI;
using CommunityToolkit.Mvvm.Input;
using System.Reactive;
using System.Linq;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace EasyFlow.Features.Dashboard;

public partial class DashboardViewModel : PageViewModelBase
{
    private readonly ITagService _tagService;
    private readonly ISessionService _sessionService;
    private CompositeDisposable? _disposables;

    [ObservableProperty]
    private Tag? _selectedTag;

    [ObservableProperty]
    private SessionType? _selectedSessionType;

    [ObservableProperty]
    private string _infoTitle;

    [ObservableProperty]
    private bool _isPlotVisible;

    [ObservableProperty]
    private FilterPeriod _selectedFilterPeriod = FilterPeriod.Days7;

    public DashboardViewModel(
        ITagService tagService,
        ISessionService sessionService)
        : base("Dashboard", Material.Icons.MaterialIconKind.ChartBar, (int)PageOrder.Dashboard)
    {
        _tagService = tagService;
        _sessionService = sessionService;
        
        _selectedSessionType = SessionType.Focus;

        SessionTypes.Add(SessionType.Focus);
        SessionTypes.Add(SessionType.Break);
        SessionTypes.Add(SessionType.LongBreak);

        var filters = FilterPeriod.Filters;
        foreach (var filter in filters)
        {
            FilterPeriods.Add(filter);
        }

        SeriePlot = [];

        this.WhenAnyValue(vm => vm.SelectedTag, 
                vm => vm.SelectedSessionType,
                vm => vm.SelectedFilterPeriod)
            .Skip(1)
            .WhereNotNull()
            .Throttle(TimeSpan.FromMilliseconds(500))
            .Select(_ => Unit.Default)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Do(_ =>
            {
                if (SelectedTag is not null && SelectedSessionType is not null)
                {
                    InfoTitle = $"{SelectedTag.Name} - {SelectedSessionType} - {SelectedFilterPeriod.Text}";
                }
                else
                {
                    InfoTitle = "Change the controls to see the info";
                }
            })
            .InvokeCommand(ReloadPlotCommand);
    }
    [ObservableProperty]
    private ISeries[]? _seriePlot;

    public Axis[] XAxes { get; set; } =
    {
        new DateTimeAxis(TimeSpan.FromDays(1), date => date.ToString("MMM dd yyyy"))
    };
    public Axis[] YAxes { get; set; } =
    {
        new Axis
        {
            Name = "Sessions (Minutes)",
            NameTextSize = 16,
            NamePaint = new SolidColorPaint
            {
                Color = SKColors.Gray,
                SKFontStyle = new SKFontStyle(SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright)
            }
        }
    };

    public ObservableCollection<Tag> Tags { get; } = [];
    public ObservableCollection<SessionType> SessionTypes { get; } = [];
    public ObservableCollection<FilterPeriod> FilterPeriods { get; } = [];
    protected override void OnActivated()
    {
        Debug.WriteLine("Activated DashboardViewModel");

        _disposables ??= [];
        
        Observable
            .StartAsync(LoadTags)
            .Subscribe(tags =>
            {
                Tags.Clear();
                foreach (var tag in tags)
                {
                    Tags.Add(tag);
                }

                SelectedTag = Tags[0];
            })
            .DisposeWith(_disposables);
    }

    protected override void OnDeactivated()
    {
        Debug.WriteLine("Deactivated DashboardViewModel");
    }

    private async Task<List<Tag>> LoadTags()
    {
        var result = _tagService.GetAll();
        if (result.Error is not null)
        {
            await SukiHost.ShowToast("Failed to load", "Failed to load the tags");
            return [];
        }

        return result.Value!;
    }

    [RelayCommand]
    private async Task ReloadPlot()
    {
        var result = await _sessionService.GetSessionsByPeriod(SelectedFilterPeriod);

        if (result.Error is not null)
        {
            await SukiHost.ShowToast("Failed to get sessions", result.Error.Message!, SukiUI.Enums.NotificationType.Error);
            return;
        }

        var sessions = result.Value!;

        var selectedSessionsByType =
            SelectedSessionType is null ?
            sessions :
            sessions.Where(s => s.SessionType == SelectedSessionType);

        var selectedSessionsByTag =
            SelectedTag is null ?
            selectedSessionsByType :
            selectedSessionsByType.Where(s => s.TagId == SelectedTag.Id);

        var sessionSummaries = selectedSessionsByTag
            .GroupBy(s => s.FinishedDate)
            .Select(group =>
            {
                var duration = group.Sum(s => s.DurationMinutes);
                var date = group.First().FinishedDate;
                DateTime newDateTime = new(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Utc);
                return new DateTimePoint(newDateTime, duration);
            });

        var selectedSessions = sessionSummaries;

        IsPlotVisible = true;
        if (!selectedSessions.Any())
        {
            await SukiHost.ShowToast("There are no sessions", "No sessions found for these settings.");
            IsPlotVisible = false;
        }

        var values = new ObservableCollection<DateTimePoint>();

        foreach (var session in selectedSessions)
        {
            values.Add(session);
        }

        var columnSeries = new ColumnSeries<DateTimePoint>
        {
            Values = values,
            YToolTipLabelFormatter = point => $"{point?.Model?.Value} minutes",
        };

        SeriePlot = [ columnSeries ];

        Debug.WriteLine("Reloaded plot");
    }
}
