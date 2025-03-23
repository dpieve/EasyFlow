using DesktopNotifications;
using DesktopNotifications.FreeDesktop;
using EasyFlow.Domain.Services;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EasyFlow.Linux;

public sealed class NotificationDesktop : INotificationService
{
    private readonly INotificationManager _notificationManager;

    public NotificationDesktop(INotificationManager? notificationManager = null)
    {
        _notificationManager = notificationManager ?? new FreeDesktopNotificationManager();
        try
        {
            _notificationManager.Initialize();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    public async Task ShowNotification(string title, string message)
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
            Debug.WriteLine(ex.Message);
        }
    }

    public void Dispose()
    {
        _notificationManager.Dispose();
    }
}