using DesktopNotifications;
using DesktopNotifications.FreeDesktop;
using EasyFocus.Domain.Services;
using Serilog;
using System;

using System.Threading.Tasks;

namespace EasyFocus.Linux;

public sealed class NotificationService : INotificationService
{
    private readonly INotificationManager _notificationManager;

    public NotificationService(INotificationManager? notificationManager = null)
    {
        _notificationManager = notificationManager ?? new FreeDesktopNotificationManager();
        try
        {
            _notificationManager.Initialize();
        }
        catch (Exception ex)
        {
            Log.Debug(ex.Message);
        }
    }

    public async Task Show(string title, string message)
    {
        try
        {
            var notification = new Notification
            {
                Title = title,
                Body = message
            };

            await _notificationManager.ShowNotification(notification);
        }
        catch (Exception ex)
        {
            Log.Debug(ex.Message);
        }
    }

    public void Dispose()
    {
        _notificationManager.Dispose();
    }
}