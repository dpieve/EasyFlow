namespace EasyFocus.Domain.Entities;

public sealed class AppSettings
{
    public int Id { get; set; }
    public Tag SelectedTag { get; set; }
    public int SelectedPomodoro { get; set; }
    public int SelectedShortBreak { get; set; }
    public int SelectedLongBreak { get; set; }
    public int PomodorosBeforeLongBreak { get; set; }
    public bool AutoStartPomodoros { get; set; }
    public bool AutoStartBreaks { get; set; }
    public bool SaveSkippedSessions { get; set; }
    public bool NotificationOnCompletion { get; set; }
    public bool NotificationAfterSkippedSessions { get; set; }
    public int AlarmVolume { get; set; }
    public Sound AlarmSound { get; set; }
    public string BackgroundPath { get; set; }
    public bool ShowTodaySessions { get; set; }

    public AppSettings(int id, Tag selectedTag, int selectedPomodoro, int selectedShortBreak, int selectedLongBreak,
        int pomodorosBeforeLongBreak, bool autoStartPomodoros, bool autoStartBreaks, bool saveSkippedSessions,
        bool notificationOnCompletion, bool notificationAfterSkippedSessions, int alarmVolume, Sound alarmSound, string backgroundPath, bool showTodaySessions)
    {
        Id = id;
        SelectedTag = selectedTag;
        SelectedPomodoro = selectedPomodoro;
        SelectedShortBreak = selectedShortBreak;
        SelectedLongBreak = selectedLongBreak;
        PomodorosBeforeLongBreak = pomodorosBeforeLongBreak;
        AutoStartPomodoros = autoStartPomodoros;
        AutoStartBreaks = autoStartBreaks;
        SaveSkippedSessions = saveSkippedSessions;
        NotificationOnCompletion = notificationOnCompletion;
        NotificationAfterSkippedSessions = notificationAfterSkippedSessions;
        AlarmVolume = alarmVolume;
        AlarmSound = alarmSound;
        BackgroundPath = backgroundPath;
        ShowTodaySessions = showTodaySessions;
    }
}