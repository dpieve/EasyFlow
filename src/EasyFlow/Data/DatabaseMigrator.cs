using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace EasyFlow.Data;
public class DatabaseMigrator
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public DatabaseMigrator(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task MigrateDatabaseAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        await context.Database.MigrateAsync();
    }

    public void MigrateDatabase()
    {
        using var context = _contextFactory.CreateDbContext();
        context.Database.EnsureDeleted();
        context.Database.Migrate();
    }
}
