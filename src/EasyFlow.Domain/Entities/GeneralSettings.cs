﻿namespace EasyFlow.Domain.Entities;

public sealed class GeneralSettings
{
    public GeneralSettings()
    {
    }

    public int Id { get; set; }
    public bool IsWorkSoundEnabled { get; set; } = true;
    public bool IsBreakSoundEnabled { get; set; } = true;
    public int WorkDurationMinutes { get; set; } = 25;
    public int BreakDurationMinutes { get; set; } = 5;
    public int LongBreakDurationMinutes { get; set; } = 10;
    public int WorkSessionsBeforeLongBreak { get; set; } = 5;
    public Theme SelectedTheme { get; set; } = Theme.Dark;
    public ColorTheme SelectedColorTheme { get; set; } = ColorTheme.Red;
    public int SelectedTagId { get; set; }
    public Tag? SelectedTag { get; set; }
    public bool IsFocusDescriptionEnabled { get; set; } = true;
    public int SoundVolume { get; set; } = 50;
    public string SelectedLanguage { get; set; } = string.Empty;
    public SessionType DashboardSessionType { get; set; } = SessionType.Focus;
    public int DashboardFilterPeriod { get; set; } = 7;
    public int DashboardDisplayType { get; set; } = 0;
}

public enum Theme
{
    Dark,
    Light
}

public enum ColorTheme
{
    Orange,
    Red,
    Green,
    Blue
}