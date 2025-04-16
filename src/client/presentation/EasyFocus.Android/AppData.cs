using EasyFocus.Domain.Entities;
using EasyFocus.Domain.Services;
using Serilog;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;

namespace EasyFocus.Android;

public sealed class AppDataJson
{
    private const string _filePath = "EasyFocus.json";
    private int _nextSessionId = 1;
    private int _nextTagId = 1;

    public AppDataJson()
    {
    }

    public List<Session> Sessions { get; set; } = new();
    public List<Tag> Tags { get; set; } = new();
    public AppSettings Settings { get; set; } = null!;

    public void LoadData(List<Tag> defaultTags, AppSettings defaultSettings)
    {
        try
        {
            //if (File.Exists(_filePath))
            //{
            //    var jsonData = File.ReadAllText(_filePath);
            //    var appData = JsonSerializer.Deserialize<AppDataJson>(jsonData);
            //    Sessions = appData?.Sessions ?? [];
            //    Tags = appData?.Tags ?? [];
            //    Settings = appData?.Settings ?? defaultSettings;
            //    _nextSessionId = Sessions.Count != 0 ? Sessions.Max(s => s.Id) + 1 : 1;
            //    _nextTagId = Tags.Count != 0 ? Tags.Max(t => t.Id) + 1 : 1;
            //}
            //else
            //{
            Sessions = new();
            Tags = [.. defaultTags];
            Settings = defaultSettings;

            SaveData();
            //}
        }
        catch (Exception ex)
        {
            Log.Debug(ex.Message);
        }
    }

    public void SaveData()
    {
        //try
        //{
        //    var options = new JsonSerializerOptions
        //    {
        //        WriteIndented = true,
        //    };
        //    var jsonData = JsonSerializer.Serialize(this, options);
        //    File.WriteAllText(_filePath, jsonData);
        //}
        //catch (Exception ex)
        //{
        //    Log.Debug(ex.Message);
        //}
    }

    public int GetNextSessionId() => _nextSessionId++;

    public int GetNextTagId() => _nextTagId++;
}

public sealed class SessionServiceJson : ISessionService
{
    private readonly AppDataJson _appData;

    public SessionServiceJson(AppDataJson appData)
    {
        _appData = appData;
    }

    public Task<Session> AddAsync(Session session)
    {
        session.Id = _appData.GetNextSessionId();
        _appData.Sessions.Add(session);
        _appData.SaveData();
        return Task.FromResult(session);
    }

    public Task<bool> DeleteAsync(Session session)
    {
        var result = _appData.Sessions.Remove(session);
        _appData.SaveData();
        return Task.FromResult(result);
    }

    public Task<Session?> GetSessionAsync(int id)
    {
        var session = _appData.Sessions.Find(s => s.Id == id);
        return Task.FromResult(session);
    }

    public Task<List<Session>> GetSessionsAsync()
    {
        return Task.FromResult(_appData.Sessions);
    }

    public Task<bool> UpdateAsync(Session session)
    {
        var index = _appData.Sessions.FindIndex(s => s.Id == session.Id);
        if (index >= 0)
        {
            _appData.Sessions[index] = session;
            _appData.SaveData();
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
}

public sealed class TagServiceJson : ITagService
{
    private readonly AppDataJson _appData;

    public TagServiceJson(AppDataJson appData)
    {
        _appData = appData;
    }

    public Task<Tag> AddTagAsync(Tag tag)
    {
        tag.Id = _appData.GetNextTagId();
        _appData.Tags.Add(tag);
        _appData.SaveData();
        return Task.FromResult(tag);
    }

    public Task<bool> DeleteTagAsync(Tag tag)
    {
        var result = _appData.Tags.Remove(tag);
        _appData.SaveData();
        return Task.FromResult(result);
    }

    public bool DeleteTag(Tag tag)
    {
        var result = _appData.Tags.Remove(tag);
        _appData.SaveData();
        return true;
    }

    public Task<Tag?> GetTagAsync(int id)
    {
        var tag = _appData.Tags.Find(t => t.Id == id);
        return Task.FromResult(tag);
    }

    public Task<List<Tag>> GetTagsAsync()
    {
        return Task.FromResult(_appData.Tags);
    }

    public Task<bool> UpdateTagAsync(Tag tag)
    {
        var index = _appData.Tags.FindIndex(t => t.Id == tag.Id);
        if (index >= 0)
        {
            _appData.Tags[index] = tag;
            _appData.SaveData();
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
}

public sealed class SettingsServiceJson : ISettingsService
{
    private readonly AppDataJson _appData;

    public SettingsServiceJson(AppDataJson appData)
    {
        _appData = appData;
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
        return Task.FromResult<AppSettings?>(_appData.Settings);
    }

    public Task Initialize()
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

        _appData.LoadData(defaultTags, defaultSettings);
        return Task.CompletedTask;
    }

    public Task<bool> UpdateSettingsAsync(AppSettings settings)
    {
        _appData.Settings = settings;
        _appData.SaveData();
        return Task.FromResult(true);
    }
}