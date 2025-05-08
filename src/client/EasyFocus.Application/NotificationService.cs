using EasyFocus.Domain.Services;

namespace EasyFocus.Application;

public sealed class NotificationService : INotificationService
{
    private readonly IAppHelpersApi _api;

    public NotificationService(IAppHelpersApi api)
    {
        _api = api;
    }

    public async Task Show(string title, string message)
    {
        await _api.ShowNotification(title, message);
    }
}