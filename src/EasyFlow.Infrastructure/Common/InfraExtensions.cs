using EasyFlow.Domain.Entities;
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
        return services;
    }
}