using EasyFlow.Domain.Entities;
using EasyFlow.Domain.Repositories;
using EasyFlow.Infrastructure.Repositories;
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

        services.AddScoped<ITagsRepository, TagsRepository>();
        services.AddScoped<IDatabaseManagerRepository, DatabaseManagerRepository>();

        return services;
    }
}
