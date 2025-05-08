using EasyFocus.Domain.Entities;
using EasyFocus.Domain.Repositories;
using EasyFocus.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EasyFocus.Windows;

public sealed class AppRepository : IAppRepository
{
    private readonly string _key = "EasyFocus";

    [JsonIgnore]
    private readonly IAppHelpersApi _api = null!;

    private int _nextSessionId = 1;
    private int _nextTagId = 1;

    [JsonConstructor]
    public AppRepository()
    {
    }

    public AppRepository(IAppHelpersApi api)
    {
        _api = api;
    }

    public bool Loaded = false;
    public List<Session> Sessions { get; set; } = [];
    public List<Tag> Tags { get; set; } = [];
    public AppSettings? Settings { get; set; }

    public async Task LoadData(List<Tag> defaultTags, AppSettings defaultSettings)
    {
        try
        {
            var jsonData = await _api.GetStorageItem(_key);

            if (!string.IsNullOrEmpty(jsonData))
            {
                var appData = JsonSerializer.Deserialize(jsonData, AppContextRepository.Default.AppRepository);

                Sessions = appData?.Sessions ?? [];
                Tags = appData?.Tags ?? [];
                Settings = appData?.Settings ?? defaultSettings;
                _nextSessionId = Sessions.Count != 0 ? Sessions.Max(s => s.Id) + 1 : 1;
                _nextTagId = Tags.Count != 0 ? Tags.Max(t => t.Id) + 1 : 1;
            }
            else
            {
                Sessions = [];
                Tags = [.. defaultTags];
                Settings = defaultSettings;

                await SaveData();
            }
        }
        catch (Exception ex)
        {
            await _api.LogValue(ex.Message);
        }
        finally
        {
            Loaded = true;
        }
    }

    public async Task SaveData()
    {
        var jsonData = JsonSerializer.Serialize(this, AppContextRepository.Default.AppRepository);
        await _api.SetStorageItem(_key, jsonData);
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

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(AppRepository))]
[JsonSerializable(typeof(Session))]
[JsonSerializable(typeof(Tag))]
[JsonSerializable(typeof(AppSettings))]
public partial class AppContextRepository : JsonSerializerContext
{
}