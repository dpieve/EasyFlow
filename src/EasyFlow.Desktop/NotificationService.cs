using DesktopNotifications;
using DesktopNotifications.FreeDesktop;
using DesktopNotifications.Windows;
using EasyFlow.Desktop.Services;
using System.Runtime.InteropServices;

namespace EasyFlow.Desktop;

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