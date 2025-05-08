using DesktopNotifications;
using DesktopNotifications.Windows;
using EasyFocus.Domain.Services;
using NetCoreAudio;
using System;
using System.Threading.Tasks;

namespace EasyFocus.Windows;

public class AppHelpersApi : IAppHelpersApi
{
    private readonly INotificationManager _notificationManager;

    public AppHelpersApi(INotificationManager? notificationManager = null)
    {
        _notificationManager = notificationManager ?? new WindowsNotificationManager();
        _notificationManager.Initialize();
    }

    public Task LogValue(string message)
    {
        return Task.CompletedTask;
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
        catch (Exception)
        {
        }
    }

    public async Task PlayAudio(string audioFileName, int volume)
    {
        try
        {
            var player = new Player();
            await player.SetVolume((byte)volume);

            var fileName = $"Assets/{audioFileName}";

            if (player.Playing)
            {
                return;
            }

            await player.Play(fileName);

            await Task.Delay(3000);
        }
        catch (Exception)
        {
        }
    }

    public Task SetBrowserTitle(string title)
    {
        return Task.CompletedTask;
    }

    public Task<bool> OpenUrl(string url)
    {
        return Task.FromResult(true);
    }

    public async Task<string> GetStorageItem(string key)
    {
        return string.Empty;
    }

    public async Task SetStorageItem(string key, string value)
    {
    }
}