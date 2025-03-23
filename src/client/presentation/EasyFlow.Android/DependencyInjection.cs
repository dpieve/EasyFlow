using EasyFlow.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EasyFlow.Android;

public static class DependencyInjection
{
    public static IServiceCollection AddBrowser(this IServiceCollection services)
    {
        services.AddSingleton<IPlaySoundService, PlaySoundAndroid>();
        services.AddSingleton<INotificationService, NotificationAndroid>();
        return services;
    }
}
