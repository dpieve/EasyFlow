using EasyFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EasyFlow.Infrastructure.Common;
public sealed class Seed
{
    public static async Task SeedData(DataContext context)
    {
        
        await SeedTags(context);
        await SeedGeneralSettings(context);
    }

    private static async Task SeedTags(DataContext context)
    {
        var has = await context.Tags.AnyAsync();
        if (has)
        {
            return;
        }

        var tags = new List<Tag>
        {
            new Tag { Name = "Work" },
            new Tag { Name = "Study" },
            new Tag { Name = "Meditate" },
            new Tag { Name = "Exercises" },
        };

        await context.Tags.AddRangeAsync(tags);
        await context.SaveChangesAsync();
    }

    private static async Task SeedGeneralSettings(DataContext context)
    {
        var has = await context.GeneralSettings.AnyAsync();
        if (has)
        {
            return;
        }

        var settings = new GeneralSettings
           {
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
           };

        await context.GeneralSettings.AddAsync(settings);
        await context.SaveChangesAsync();
    }
}