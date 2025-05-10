using EasyFocus.Domain.Entities;
using EasyFocus.Domain.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyFocus.Resources.Mockups;

public class AppSettingsServiceMockup : IAppSettingsService
{
    private readonly List<AppSettings> _settings = new();
    private int _nextId = 1;

    public Task<AppSettings> AddSettingsAsync(AppSettings settings)
    {
        settings.Id = _nextId++;
        _settings.Add(settings);
        return Task.FromResult(settings);
    }

    public bool DeleteSettings(AppSettings settings)
    {
        return _settings.Remove(settings);
    }

    public Task<bool> DeleteSettingsAsync(AppSettings settings)
    {
        bool removed = _settings.Remove(settings);
        return Task.FromResult(removed);
    }

    public Task<List<AppSettings>> GetSettingsAsync()
    {
        return Task.FromResult(_settings.ToList());
    }

    public Task<AppSettings?> GetSettingsAsync(int id)
    {
        var found = _settings.FirstOrDefault(s => s.Id == id);
        return Task.FromResult(found);
    }

    public Task Initialize()
    {
        _settings.Add(LivePreviewMockup.AppSettings);
        return Task.CompletedTask;
    }

    public Task<bool> UpdateSettingsAsync(AppSettings settings)
    {
        var index = _settings.FindIndex(s => s.Id == settings.Id);
        if (index == -1)
        {
            return Task.FromResult(false);
        }

        _settings[index] = settings;
        return Task.FromResult(true);
    }
}