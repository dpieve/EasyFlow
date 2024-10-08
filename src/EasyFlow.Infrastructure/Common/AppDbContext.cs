﻿using EasyFlow.Domain.Entities;
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
public class AppDbContext : DbContext
{
    public DbSet<GeneralSettings> GeneralSettings { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Session> Sessions { get; set; }

    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected AppDbContext(DbContextOptions options)
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tag>()
            .HasData(
                new Tag { Id = 1, Name = "Work" },
                new Tag { Id = 2, Name = "Study" },
                new Tag { Id = 3, Name = "Meditate" },
                new Tag { Id = 4, Name = "Exercises" });

        modelBuilder.Entity<GeneralSettings>()
            .HasData(
                new GeneralSettings
                {
                    Id = 1,
                    IsWorkSoundEnabled = true,
                    IsBreakSoundEnabled = true,
                    WorkDurationMinutes = 25,
                    BreakDurationMinutes = 5,
                    LongBreakDurationMinutes = 10,
                    WorkSessionsBeforeLongBreak = 4,
                    SelectedTheme = Theme.Dark,
                    SelectedColorTheme = ColorTheme.Red,
                    SelectedTagId = 1,
                    SelectedLanguage = "en",
                });
    }
}