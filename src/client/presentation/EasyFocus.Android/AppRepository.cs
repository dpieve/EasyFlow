using EasyFocus.Domain.Entities;
using EasyFocus.Domain.Repositories;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyFocus.Android;

public sealed class AppRepository : IAppRepository
{
    private int _nextSessionId = 1;
    private int _nextTagId = 1;

    public AppRepository()
    {
    }

    public List<Session> Sessions { get; set; } = [];
    public List<Tag> Tags { get; set; } = [];
    public AppSettings? Settings { get; set; }

    public async Task LoadData(List<Tag> defaultTags, AppSettings defaultSettings)
    {
        try
        {
            Sessions = [];
            Tags = [.. defaultTags];
            Settings = defaultSettings;

            await SaveData();
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
        }
    }

    public Task SaveData()
    {
        return Task.CompletedTask;
    }

    public int GetNextSessionId() => _nextSessionId++;

    public int GetNextTagId() => _nextTagId++;

    public AppSettings? GetSettings()
    {
        return Settings;
    }

    public void SetSettings(AppSettings settings)
    {
        Settings = settings;
    }

    public List<Session> GetSessions()
    {
        return Sessions;
    }

    public void AddSession(Session session)
    {
        Sessions.Add(session);
    }

    public void UpdateSession(int index, Session session)
    {
        if (index < 0 || index >= Sessions.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        }
        Sessions[index] = session;
    }

    public bool DeleteSession(Session session)
    {
        return Sessions.Remove(session);
    }

    public List<Tag> GetTags()
    {
        return Tags;
    }

    public void AddTag(Tag tag)
    {
        Tags.Add(tag);
    }

    public void UpdateTag(int index, Tag tag)
    {
        if (index < 0 || index >= Tags.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        }

        Tags[index] = tag;
    }

    public bool DeleteTag(Tag tag)
    {
        return Tags.Remove(tag);
    }
}