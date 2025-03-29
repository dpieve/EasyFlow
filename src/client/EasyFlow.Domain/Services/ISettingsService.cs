using EasyFlow.Domain.Entities;

namespace EasyFlow.Domain.Services;

public interface ISettingsService
{
    public Task<List<AppSettings>> GetSettingsAsync();
    public Task<AppSettings?> GetSettingsAsync(int id);
    public Task<bool> DeleteSettingsAsync(AppSettings settings);
    public bool DeleteSettings(AppSettings settings);
    public Task<AppSettings> AddSettingsAsync(AppSettings settings);
    public Task<bool> UpdateSettingsAsync(AppSettings settings);

    public Task Initialize();
}