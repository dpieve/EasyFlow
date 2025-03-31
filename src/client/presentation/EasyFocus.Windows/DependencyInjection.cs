using EasyFocus.Domain.Services;
using EasyFocus.Services.Desktop;
using Microsoft.Extensions.DependencyInjection;

namespace EasyFocus.Windows;

public static class DependencyInjection
{
    public static IServiceCollection AddPlatform(this IServiceCollection services)
    {
        services.AddSingleton<IPlaySoundService, PlaySoundDesktop>();
        services.AddSingleton<INotificationService, NotificationDesktop>();

        services.AddSingleton<AppDataJson>();
        services.AddSingleton<ITagService, TagServiceJson>();
        services.AddSingleton<ISessionService, SessionServiceJson>();
        services.AddSingleton<ISettingsService, SettingsServiceJson>();
        return services;
    }
}