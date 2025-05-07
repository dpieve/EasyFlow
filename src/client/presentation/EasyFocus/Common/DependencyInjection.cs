using EasyFocus.Domain.Services;
using EasyFocus.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EasyFocus.Common;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddSingleton<IBrowserService, BrowserService>();
        return services;
    }
}