using Avalonia.Controls;
using Avalonia.Controls.Templates;
using EasyFocus.Features.Pomodoro;
using EasyFocus.Features.Report;
using EasyFocus.Features.Settings;
using EasyFocus.Features.Settings.Background;
using EasyFocus.Features.Settings.FocusTime;
using EasyFocus.Features.Settings.HomeSettings;
using EasyFocus.Features.Settings.Notifications;
using EasyFocus.Features.Settings.Tags;
using System.Collections.Generic;
using System.ComponentModel;

namespace EasyFocus.Common;

public class ViewLocator : IDataTemplate
{
    private readonly Dictionary<string, Control> _controlCache = [];

    public Control Build(object? data)
    {
        if (data is null)
        {
            return new TextBlock { Text = "Data is null." };
        }

        string? name = data.GetType().Name;

        if (name is null)
        {
            return new TextBlock { Text = "Name is null." };
        }

        if (_controlCache.TryGetValue(name, out var res))
        {
            return res;
        }

        res = name switch
        {
            nameof(PomodoroViewModel) => new PomodoroView() { DataContext = data },
            nameof(ReportViewModel) => new ReportView() { DataContext = data },
            nameof(BackgroundViewModel) => new BackgroundView() { DataContext = data },
            nameof(FocusTimeViewModel) => new FocusTimeView() { DataContext = data },
            nameof(HomeSettingsViewModel) => new HomeSettingsView() { DataContext = data },
            nameof(NotificationsViewModel) => new NotificationsView() { DataContext = data },
            nameof(TagsViewModel) => new TagsView() { DataContext = data },
            nameof(SettingsViewModel) => new SettingsView() { DataContext = data },
            _ => new TextBlock { Text = $"No View For {name}." },
        };

        _controlCache.Add(name, res);

        return res;
    }

    public bool Match(object? data) => data is INotifyPropertyChanged;
}