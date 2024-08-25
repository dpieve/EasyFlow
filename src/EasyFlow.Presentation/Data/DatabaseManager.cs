using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace EasyFlow.Presentation.Data;

public interface IDatabaseManager
{
    public Task MigrateAsync();
    public void Migrate();
    public bool Reset();
}

public class DatabaseManager : IDatabaseManager
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public DatabaseManager(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task MigrateAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        await context.Database.MigrateAsync();
    }

    public void Migrate()
    {
        using var context = _contextFactory.CreateDbContext();
        context.Database.Migrate();
    }

    public bool Reset()
    {
        using var context = _contextFactory.CreateDbContext();
        var result = context.Database.EnsureDeleted();
        context.Database.Migrate();
        return result;
    }
}
