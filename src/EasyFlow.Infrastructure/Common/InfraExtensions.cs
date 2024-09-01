using EasyFlow.Domain.Entities;
using EasyFlow.Domain.Repositories;
using EasyFlow.Domain.Services;
using EasyFlow.Infrastructure.Repositories;
using EasyFlow.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EasyFlow.Infrastructure.Common;

public static class InfraExtensions
{
    public static ServiceCollection AddInfrastructure(this ServiceCollection services)
    {
        services.AddDbContextFactory<AppDbContext>(
            options =>
            {
                options.UseSqlite($"Data Source={Paths.DbFullPath}");
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
            },
            lifetime: ServiceLifetime.Scoped);

        // Services
        services.AddScoped<IPlaySoundService, PlaySoundService>();

        // Repositories
        services.AddScoped<ITagsRepository, TagsRepository>()
                .AddScoped<IGeneralSettingsRepository, GeneralSettingsRepository>()
                .AddScoped<IDatabaseManagerRepository, DatabaseManagerRepository>()
                .AddScoped<ISessionsRepository, SessionsRepository>();

        return services;
    }
}