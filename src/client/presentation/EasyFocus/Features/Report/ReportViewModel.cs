using DynamicData;
using EasyFocus.Common;
using EasyFocus.Domain.Entities;
using EasyFocus.Domain.Services;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace EasyFocus.Features.Report;

public sealed partial class ReportViewModel : ViewModelBase
{
    [Reactive] private int _totalFocusSeconds;
    [Reactive] private int _totalBreakSeconds;
    [Reactive] private int _totalSessions;

    [Reactive] private FilterPeriod _selectedFilterPeriod = FilterPeriod.Hours48;
    [Reactive] private SessionType _selectedSessionType;
    [Reactive] private string _filterText = string.Empty;

    [Reactive] private SessionItemViewModel? _selectedSessionRow;
    private readonly ISessionService _sessionService;

    public ReportViewModel(ISessionService sessionService)
    {
        _sessionService = sessionService;

        SessionTypes.AddRange(Enum.GetValues<SessionType>().Cast<SessionType>());

        this.WhenAnyValue(vm => vm.SelectedSessionType,
                              vm => vm.SelectedFilterPeriod,
                              vm => vm.FilterText)
                 .Select(_ => CreateDisplaySettings())
                 .Select(displaySettings => Observable.FromAsync(() => LoadData(displaySettings)))
                 .Concat()
                 .Subscribe(Update);
    }

    public ObservableCollection<SessionItemViewModel> Sessions { get; } = [];
    public ObservableCollection<SessionType> SessionTypes { get; } = [];

    [ReactiveCommand]
    public async Task Reload()
    {
        var sessions = await LoadData(CreateDisplaySettings());
        Update(sessions);
    }

    [ReactiveCommand]
    private void OnBack()
    {
        Log.Debug("Report OnBack");
    }

    private void Update(List<Session> sessions)
    {
        Sessions.Clear();

        foreach (var session in sessions)
        {
            Sessions.Add(new SessionItemViewModel(session, _sessionService));
        }

        TotalFocusSeconds = Sessions.Where(s => s.Session.SessionType == SessionType.Pomodoro)
            .Sum(s => s.Session.CompletedSeconds);

        TotalBreakSeconds = Sessions.Where(s => s.Session.SessionType != SessionType.Pomodoro)
            .Sum(s => s.Session.CompletedSeconds);

        TotalSessions = Sessions.Where(s => s.SessionType == SessionType.Pomodoro).Count();
    }

    private DisplaySettings CreateDisplaySettings()
    {
        return new DisplaySettings(SelectedSessionType, SelectedFilterPeriod, FilterText);
    }

    private async Task<List<Session>> LoadData(DisplaySettings displaySettings)
    {
        var sessionType = displaySettings.SessionType;
        var filterPeriod = displaySettings.FilterPeriod;

        var startDate = DateTime.Now.AddDays(-filterPeriod.NumDays);

        var sessionsData = await _sessionService.GetSessionsAsync();

        var selectedSessions = sessionsData
            .Where(s => sessionType == SessionType.None || s.SessionType == sessionType)
            .Where(s => s.FinishedDateTime >= startDate)
            .Where(s => s.ApplyFilter(displaySettings.FilterText))
            .OrderByDescending(s => s.FinishedDateTime)
            .ToList();

        return selectedSessions;
    }
}