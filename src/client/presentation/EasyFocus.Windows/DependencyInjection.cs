using EasyFocus.Domain.Repositories;
using EasyFocus.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EasyFocus.Windows;

public static class DependencyInjection
{
    public static IServiceCollection AddPlatform(this IServiceCollection services)
    {
        services.AddSingleton<IAppHelpersApi, AppHelpersApi>();
        services.AddSingleton<IAppRepository, AppRepository>();
        return services;
    }
}