using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyFlow.Common;
using EasyFlow.Data;
using EasyFlow.Features.Focus.RunningTimer;
using EasyFlow.Services;
using SimpleRouter;
using SukiUI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace EasyFlow.Features.Focus.AdjustTimers;

public sealed partial class AdjustTimersViewModel : ViewModelBase, IRoute, IActivatableRoute
{
    private const int _deltaWorkTime = 5;
    private const int _deltaBreakTime = 1;
    private const int _deltaLongBreakTime = 1;

    private const int _maxWorkTime = 40;
    private const int _minWorkTime = 1;

    private const int _maxBreakTime = 40;
    private const int _minBreakTime = 1;

    private const int _maxLongBreakTime = 40;
    private const int _minLongBreakTime = 1;

    private readonly ITagService _tagService;

    private int _timesBeforeLongBreak = 5;

    [ObservableProperty]
    private int _workTimeMinutes = 25;

    [ObservableProperty]
    private int _breakTimeMinutes = 5;

    [ObservableProperty]
    private int _longBreakTimeMinutes = 10;

    [ObservableProperty]
    private Tag? _selectedTag;

    [ObservableProperty]
    private bool _isStartLoading;

    public AdjustTimersViewModel(IRouterHost routerHost, ITagService? tagService = null)
    {
        RouterHost = routerHost ?? throw new ArgumentNullException(nameof(routerHost));
        _tagService = tagService ?? throw new ArgumentNullException(nameof(tagService));
    }

    public ObservableCollection<Tag> Tags { get; } = new();

    void IActivatableRoute.OnActivated()
    {
        var result = _tagService.GetAll();
        if (result.Error is not null)
        {
            SukiHost.ShowToast("Failed to load", "Failed to load the tags");
        }
        else
        {
            var tags = result.Value!;
            Reload(tags);

            SelectedTag = tags.FirstOrDefault();
        }

        Debug.WriteLine("Activated AdjustTimersViewModel");
    }

    void IActivatableRoute.OnDeactivated()
    {
        Debug.WriteLine("Deactivated AdjustTimersViewModel");
    }

    public string RouteName => nameof(AdjustTimersViewModel);

    public IRouterHost RouterHost { get; }

    [RelayCommand]
    private void NavigateToRunTimer()
    {
        var workTimeSettings = new TimerSettings(MinimumMinutes: 0, MaximumMinutes: 40, TotalMinutes: WorkTimeMinutes, DeltaMinutes: 5);
        var breakTimeSettings = new TimerSettings(MinimumMinutes: 0, MaximumMinutes: 40, TotalMinutes: BreakTimeMinutes, DeltaMinutes: 1);
        var longBreakTimeSettings = new TimerSettings(MinimumMinutes: 0, MaximumMinutes: 40, TotalMinutes: LongBreakTimeMinutes, DeltaMinutes: 1);
        var tag = SelectedTag;

        var focus = new FocusSettings(workTimeSettings, breakTimeSettings, longBreakTimeSettings, tag, _timesBeforeLongBreak);

        IsStartLoading = true;
        RouterHost.Router.NavigateTo<RunningTimerViewModel>(RouterHost, focus);
        IsStartLoading = false;
    }

    [RelayCommand(CanExecute = nameof(CanIncreaseTime))]
    private void IncreaseTime(string timeType)
    {
        UpdateTime(timeType, 1);
    }

    [RelayCommand(CanExecute = nameof(CanDecreaseTime))]
    private void DecreaseTime(string timeType)
    {
        UpdateTime(timeType, -1);
    }

    private bool CanIncreaseTime(string timeType)
    {
        var delta = GetDelta(timeType, 1);
        return CanAdjustTime(timeType, delta);
    }

    private bool CanDecreaseTime(string timeType)
    {
        var delta = GetDelta(timeType, -1);
        return CanAdjustTime(timeType, delta);
    }

    private bool CanAdjustTime(string timeType, int delta)
    {
        return timeType switch
        {
            "Work" => WorkTimeMinutes + delta >= _minWorkTime && WorkTimeMinutes + delta <= _maxWorkTime,
            "Break" => BreakTimeMinutes + delta >= _minBreakTime && BreakTimeMinutes + delta <= _maxBreakTime,
            "LongBreak" => LongBreakTimeMinutes + delta >= _minLongBreakTime && LongBreakTimeMinutes + delta <= _maxLongBreakTime,
            _ => false,
        };
    }

    private void UpdateTime(string timeType, int delta)
    {
        switch (timeType)
        {
            case "Work":
                WorkTimeMinutes += GetDelta(timeType, delta);
                break;

            case "Break":
                BreakTimeMinutes += GetDelta(timeType, delta);
                break;

            case "LongBreak":
                LongBreakTimeMinutes += GetDelta(timeType, delta);
                break;

            default:
                throw new ArgumentException("Invalid time type", nameof(timeType));
        }
    }

    private int GetDelta(string timeType, int delta)
    {
        return timeType switch
        {
            "Work" => delta * _deltaWorkTime,
            "Break" => delta * _deltaBreakTime,
            "LongBreak" => delta * _deltaLongBreakTime,
            _ => 0,
        };
    }

    [RelayCommand]
    private void OpenLongBreakSettings()
    {
        SukiHost.ShowDialog(new LongBreakSettingsViewModel(EditedLoadBreakSettings), allowBackgroundClose: false);
    }

    private void EditedLoadBreakSettings(int longBreakSessions)
    {
        _timesBeforeLongBreak = longBreakSessions;
    }

    private void Reload(List<Tag> tags)
    {
        Tags.Clear();
        foreach (var tag in tags)
        {
            Tags.Add(tag);
        }
    }
}