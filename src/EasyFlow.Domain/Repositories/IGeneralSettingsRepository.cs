using EasyFlow.Domain.Entities;

namespace EasyFlow.Domain.Repositories;
public interface IGeneralSettingsRepository
{
    public Task<List<GeneralSettings>> GetAsync();
    public Task<bool> UpdateAsync(GeneralSettings settings);
}
