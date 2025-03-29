using EasyFlow.Domain.Entities;
using EasyFlow.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace EasyFlow.Browser;

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
