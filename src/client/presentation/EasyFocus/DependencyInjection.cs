using EasyFocus.Application;
using EasyFocus.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EasyFocus;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddSingleton<IAudioService, AudioService>();
        services.AddSingleton<INotificationService, NotificationService>();
        services.AddSingleton<ITagService, TagService>();
        services.AddSingleton<ISessionService, SessionService>();
        services.AddSingleton<IAppSettingsService, AppSettingsService>();
        services.AddSingleton<IBrowserService, BrowserService>();
        return services;
    }
}