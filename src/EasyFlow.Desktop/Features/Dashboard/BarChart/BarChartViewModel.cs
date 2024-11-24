using EasyFlow.Desktop.Common;
using EasyFlow.Desktop.Services;
using EasyFlow.Domain.Entities;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using ReactiveUI.SourceGenerators;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EasyFlow.Desktop.Features.Dashboard.BarChart;

public sealed partial class BarChartViewModel : ViewModelBase
{
    private readonly ILanguageService _languageService;

    [Reactive]
    private ISeries[]? _seriePlot;

    [Reactive]
    private bool _isBarChartVisible;

    public BarChartViewModel(ILanguageService languageService)
    {
        _languageService = languageService;
        SeriePlot = [];
    }

    public Axis[] XAxes { get; set; } =
{
        new DateTimeAxis(TimeSpan.FromDays(1), date => date.ToString(LanguageService.GetDateFormat()))
        {
            LabelsPaint = new SolidColorPaint(SKColors.LightGray)
        }
    };

    public Axis[] YAxes { get; set; } =
    {
        new Axis
        {
            Name = ConstantTranslation.SessionsMinutes,
            NamePaint = new SolidColorPaint(SKColors.LightGray)
        }
    };

    public void Update(List<Session> sessions)
    {
        var sessionSummaries = sessions
            .GroupBy(s => new { s.FinishedDate.Year, s.FinishedDate.Month, s.FinishedDate.Day })
            .Select(group =>
            {
                var duration = group.Sum(s => s.DurationMinutes);
                var date = group.First().FinishedDate;
                DateTime newDateTime = new(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Utc);
                return new DateTimePoint(newDateTime, duration);
            });

        var selectedSessions = sessionSummaries;

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
    }
}