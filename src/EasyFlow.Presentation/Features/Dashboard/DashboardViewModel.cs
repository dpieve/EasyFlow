using CommunityToolkit.Mvvm.ComponentModel;
using EasyFlow.Presentation.Common;
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
using System.Linq;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using Avalonia.Controls;
using EasyFlow.Domain.Entities;
using MediatR;
using EasyFlow.Application.Tags;
using EasyFlow.Presentation.Services;
using EasyFlow.Application.Sessions;
using DynamicData;

namespace EasyFlow.Presentation.Features.Dashboard;

public partial class DashboardViewModel : PageViewModelBase
{
    private readonly IMediator _mediator;
    private readonly ILanguageService _languageService;

    private CompositeDisposable? _disposables;

    [ObservableProperty]
    private Tag? _selectedTag;

    [ObservableProperty]
    private SessionType? _selectedSessionType = SessionType.Focus;

    [ObservableProperty]
    private string _infoTitle = string.Empty;

    [ObservableProperty]
    private bool _isPlotVisible;

    [ObservableProperty]
    private bool _isPlotLoading;

    [ObservableProperty]
    private FilterPeriod _selectedFilterPeriod = FilterPeriod.Days7;

    [ObservableProperty]
    private bool _isGeneratingReport = false;
    public DashboardViewModel(
        IMediator mediator,
        ILanguageService languageService)
        : base(ConstantTranslation.SideMenuDashboard, Material.Icons.MaterialIconKind.ChartBar, (int)PageOrder.Dashboard)
    {
        _mediator = mediator;
        _languageService = languageService;
        
        IsPlotLoading = true;

        SessionTypes.AddRange(Enum.GetValues(typeof(SessionType)).Cast<SessionType>());

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
            .Select(_ => System.Reactive.Unit.Default)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Do(_ =>
            {
                if (SelectedTag is not null && SelectedSessionType is not null)
                {
                    InfoTitle = $"{SelectedTag.Name} - {SelectedSessionType.Value.ToCustomString()} - {SelectedFilterPeriod.Text}";
                }
                else
                {
                    InfoTitle = "Change the controls to see the info";
                }

                IsPlotLoading = true;
            })
            .InvokeCommand(ReloadPlotCommand);
    }

    [ObservableProperty]
    private ISeries[]? _seriePlot;

    public Axis[] XAxes { get; set; } =
    {
        new DateTimeAxis(TimeSpan.FromDays(1), date => date.ToString(LanguageService.GetDateFormat()))
    };
    public Axis[] YAxes { get; set; } =
    {
        new Axis
        {
            Name = ConstantTranslation.SessionsMinutes,
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
        SukiHost.ClearAllToasts();
    }

    private async Task<List<Tag>> LoadTags()
    {
        var result = await _mediator.Send(new GetTagsQuery());
        return result.Value;
    }

    [RelayCommand]
    private async Task FullReport()
    {
        try
        {
            IsGeneratingReport = true;
            var result = await GenerateReportHandler.Handle(_mediator);
            if (result.IsSuccess)
            {
                await SukiHost.ShowToast(_languageService.GetString("Success"), _languageService.GetString("SuccessGeneratedReport"), SukiUI.Enums.NotificationType.Success);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            await SukiHost.ShowToast(_languageService.GetString("Failure"), _languageService.GetString("FailureGeneratedReport"), SukiUI.Enums.NotificationType.Error);
        }
        finally
        {
            IsGeneratingReport = false;
        }
    }
    

    [RelayCommand]
    private async Task ReloadPlot()
    {
        var result = await _mediator.Send(new GetSessionsByPeriodQuery() { NumDays = SelectedFilterPeriod.NumDays });

        if (!result.IsSuccess)
        {
            return;
        }

        var sessions = result.Value;

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
            YToolTipLabelFormatter = point => $"{point?.Model?.Value} {_languageService.GetString("Minutes")}",
        };

        SeriePlot = [columnSeries];

        IsPlotLoading = false;
    }
}
