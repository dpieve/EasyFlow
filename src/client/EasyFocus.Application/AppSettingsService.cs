using EasyFocus.Domain.Entities;
using EasyFocus.Domain.Repositories;
using EasyFocus.Domain.Services;

namespace EasyFocus.Application;

public sealed class AppSettingsService : IAppSettingsService
{
    private readonly IAppRepository _appRepository;

    public AppSettingsService(IAppRepository appData)
    {
        _appRepository = appData;
    }

    public Task<AppSettings> AddSettingsAsync(AppSettings settings)
    {
        throw new NotImplementedException();
    }

    public bool DeleteSettings(AppSettings settings)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteSettingsAsync(AppSettings settings)
    {
        throw new NotImplementedException();
    }

    public Task<List<AppSettings>> GetSettingsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<AppSettings?> GetSettingsAsync(int id)
    {
        return Task.FromResult(_appRepository.GetSettings());
    }

    public async Task Initialize()
    {
        var defaultTags = new List<Tag>
          {
              Tag.CreateTag("Work", 1),
              Tag.CreateTag("Study", 2),
              Tag.CreateTag("Chores", 3)
          };

        var defaultSettings = new AppSettings(
            id: 1,
            selectedTag: defaultTags.First(),
            selectedPomodoro: 25,
            selectedShortBreak: 5,
            selectedLongBreak: 10,
            pomodorosBeforeLongBreak: 4,
            autoStartPomodoros: true,
            autoStartBreaks: true,
            saveSkippedSessions: false,
            notificationOnCompletion: true,
            notificationAfterSkippedSessions: false,
            alarmVolume: 50,
            alarmSound: Sound.Audio1,
            backgroundPath: "background1.png",
            showTodaySessions: true
            );

        await _appRepository.LoadData(defaultTags, defaultSettings);
    }

    public Task<bool> UpdateSettingsAsync(AppSettings settings)
    {
        _appRepository.SetSettings(settings);
        _appRepository.SaveData();
        return Task.FromResult(true);
    }
}