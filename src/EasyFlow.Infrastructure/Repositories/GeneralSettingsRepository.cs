using EasyFlow.Domain.Entities;
using EasyFlow.Domain.Repositories;
using EasyFlow.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace EasyFlow.Infrastructure.Repositories;

public sealed class GeneralSettingsRepository : IGeneralSettingsRepository
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public GeneralSettingsRepository(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<GeneralSettings>> GetAsync()
    {
        var context = await _contextFactory.CreateDbContextAsync();
        return await context.GeneralSettings.Include(gs => gs.SelectedTag).ToListAsync();
    }

    public async Task<bool> UpdateAsync(GeneralSettings settings)
    {
        var context = await _contextFactory.CreateDbContextAsync();
        context.GeneralSettings.Update(settings);
        var result = await context.SaveChangesAsync();
        return result != 0;
    }
}