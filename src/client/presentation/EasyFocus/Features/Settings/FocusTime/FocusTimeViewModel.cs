using EasyFocus.Common;
using EasyFocus.Domain.Entities;
using EasyFocus.Domain.Services;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Serilog;

using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace EasyFocus.Features.Settings.FocusTime;

public sealed partial class FocusTimeViewModel : ViewModelBase
{
    private readonly IAppSettingsService _settingsService;

    [Reactive] private int _pomodoro;
    [Reactive] private int _break;
    [Reactive] private int _longBreak;
    [Reactive] private int _pomodoroSessionsBeforeLongBreak;
    [Reactive] private bool _autoStartPomodorosEnabled;
    [Reactive] private bool _autoStartBreaksEnabled;
    [Reactive] private bool _saveProgressWhenSkippingSession;
    [Reactive] private bool _showTodaySession;

    public FocusTimeViewModel(AppSettings settings, IAppSettingsService settingsService)
    {
        Settings = settings;
        _settingsService = settingsService;

        Pomodoro = settings.SelectedPomodoro;
        Break = settings.SelectedShortBreak;
        LongBreak = settings.SelectedLongBreak;
        PomodoroSessionsBeforeLongBreak = settings.PomodorosBeforeLongBreak;
        AutoStartPomodorosEnabled = settings.AutoStartPomodoros;
        AutoStartBreaksEnabled = settings.AutoStartBreaks;
        SaveProgressWhenSkippingSession = settings.SaveSkippedSessions;
        ShowTodaySession = settings.ShowTodaySessions;

        this.WhenAnyValue(vm => vm.Pomodoro,
            vm => vm.Break,
            vm => vm.LongBreak,
            vm => vm.PomodoroSessionsBeforeLongBreak,
            vm => vm.AutoStartPomodorosEnabled,
            vm => vm.AutoStartBreaksEnabled,
            vm => vm.SaveProgressWhenSkippingSession)
            .Skip(1)
            .Select(_ => Unit.Default)
            .InvokeCommand(SaveChangesCommand);

        this.WhenAnyValue(vm => vm.ShowTodaySession)
            .Select(_ => Unit.Default)
            .Skip(1)
            .InvokeCommand(SaveChangesCommand);
    }

    public AppSettings Settings { get; set; }

    [ReactiveCommand]
    private void OnBack()
    {
        Log.Debug("Focus Time OnBack");
    }

    [ReactiveCommand]
    private async Task SaveChanges()
    {
        Settings.SelectedPomodoro = _pomodoro;
        Settings.SelectedShortBreak = _break;
        Settings.SelectedLongBreak = _longBreak;
        Settings.PomodorosBeforeLongBreak = _pomodoroSessionsBeforeLongBreak;
        Settings.AutoStartPomodoros = _autoStartPomodorosEnabled;
        Settings.AutoStartBreaks = _autoStartBreaksEnabled;
        Settings.SaveSkippedSessions = _saveProgressWhenSkippingSession;
        Settings.ShowTodaySessions = _showTodaySession;
        await _settingsService.UpdateSettingsAsync(Settings);
    }
}