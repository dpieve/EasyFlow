using EasyFocus.Api.Tags;
using EasyFocus.Api.Users;
using Microsoft.AspNetCore.Identity;

namespace EasyFocus.Api.Data;

public sealed class Seed
{
    public static async Task SeedData(AppDbContext context, UserManager<AppUser> userManager)
    {
        await UserData(userManager);
        await GeneralData(context);
    }

    private static async Task UserData(UserManager<AppUser> userManager)
    {
        if (userManager.Users.Any())
        {
            return;
        }

        var users = new List<AppUser>
        {
            new AppUser { UserName = "admin@admin.com", Email = "admin@admin.com" }
        };

        foreach (var user in users)
        {
            await userManager.CreateAsync(user, "Pa$$w0rd");
        }
    }

    private static async Task GeneralData(AppDbContext context)
    {
        if (context.Tags.Any())
        {
            return;
        }

        var tags = new List<Tag>
        {
            new Tag { Name = "Work" },
            new Tag { Name = "Study" },
            new Tag { Name = "Exercise" },
        };

        await context.Tags.AddRangeAsync(tags);
        await context.SaveChangesAsync();
    }
}