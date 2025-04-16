using DesktopNotifications;
using DesktopNotifications.Windows;
using EasyFocus.Domain.Services;
using Serilog;
using System;

using System.Threading.Tasks;

namespace EasyFocus.Windows;

public sealed class NotificationDesktop : INotificationService
{
    private readonly INotificationManager _notificationManager;

    public NotificationDesktop(INotificationManager? notificationManager = null)
    {
        _notificationManager = notificationManager ?? new WindowsNotificationManager();
        try
        {
            _notificationManager.Initialize();
        }
        catch (Exception ex)
        {
            Log.Debug(ex.Message);
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
            Log.Debug(ex.Message);
        }
    }

    public void Dispose()
    {
        _notificationManager.Dispose();
    }
}