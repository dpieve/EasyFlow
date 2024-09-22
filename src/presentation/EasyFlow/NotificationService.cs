using System.Runtime.InteropServices;
using System;
using DesktopNotifications.Windows;
using DesktopNotifications;
using System.Threading.Tasks;
using DesktopNotifications.FreeDesktop;
using EasyFlow.Desktop.Services;

namespace EasyFlow;

public sealed class NotificationService : INotificationService
{
    private readonly INotificationManager _notificationManager;

    public NotificationService(INotificationManager? notificationManager = null)
    {
        _notificationManager = notificationManager ?? CreateManager();
    }

    private static INotificationManager CreateManager()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return new FreeDesktopNotificationManager();
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return new WindowsNotificationManager();
        }

        throw new PlatformNotSupportedException();
    }

    public async Task Initialize()
    {
        await _notificationManager.Initialize();
    }

    public void Dispose()
    {
        _notificationManager.Dispose();
    }

    public async Task Show(string title, string message)
    {
        var notification = new Notification
        {
            Title = title,
            Body = message,
        };

        await _notificationManager.ShowNotification(notification);
    }
}
