using EasyFlow.Domain.Repositories;
using EasyFlow.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace EasyFlow.Infrastructure.Repositories;

public class DatabaseManagerRepository : IDatabaseManagerRepository
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public DatabaseManagerRepository(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task MigrateAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        await context.Database.MigrateAsync();
    }

    public async Task<bool> ResetAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var result = await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();

        return result;
    }

    public bool Reset()
    {
        using var context = _contextFactory.CreateDbContext();
        var result = context.Database.EnsureDeleted();
        context.Database.Migrate();
        return result;
    }
}