using EasyFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EasyFlow.Infrastructure.Common;
/// <summary>
/// Add new items to the database
/// (1) Open the Package Manager Console and go to EasyFlow.Infrastructure project.
/// (2) Run: Add-Migration \Migration-Name\
/// (3) Update the database with the following command:
/// Update-Database
/// To remove a migration: Remove-Migration
/// </summary>
public class DataContext : DbContext
{
    public DbSet<GeneralSettings> GeneralSettings { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Session> Sessions { get; set; }

    public DataContext()
    {
    }

    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }

    protected DataContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
        {
            return;
        }

        optionsBuilder.UseSqlite($"Data Source={Paths.DbFullPath}");
    }
}