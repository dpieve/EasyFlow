using Microsoft.EntityFrameworkCore;

namespace EasyFlow.Data;
public sealed class AppDbContext : DbContext
{
    public DbSet<GeneralSettings> GeneralSettings { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Session> Sessions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={App.DbFullPath}");
    }
}
