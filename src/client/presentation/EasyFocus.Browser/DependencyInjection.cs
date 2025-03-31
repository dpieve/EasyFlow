using EasyFocus.Browser;
using EasyFocus.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EasyFocus.Browser;

public static class DependencyInjection
{
    public static IServiceCollection AddBrowser(this IServiceCollection services)
    {
        services.AddSingleton<IPlaySoundService, PlaySoundBrowser>();
        services.AddSingleton<INotificationService, NotificationBrowser>();

        services.AddSingleton<AppDataJson>();
        services.AddSingleton<ITagService, TagServiceJson>();
        services.AddSingleton<ISessionService, SessionServiceJson>();
        services.AddSingleton<ISettingsService, SettingsServiceJson>();
        return services;
    }
}