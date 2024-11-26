using EasyFlow.Domain.Entities;
using EasyFlow.Domain.Services;
using EasyFlow.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EasyFlow.Infrastructure.Common;

public static class InfraExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContextFactory<DataContext>(
            options =>
            {
                options.UseSqlite($"Data Source={Paths.DbFullPath}");
#if DEBUG
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
#endif
            },
            lifetime: ServiceLifetime.Scoped);

        // Services
        services.AddScoped<IPlaySoundService, PlaySoundService>();

        return services;
    }
}