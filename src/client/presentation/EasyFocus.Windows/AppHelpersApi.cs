using DesktopNotifications;
using DesktopNotifications.Windows;
using EasyFocus.Domain.Services;
using NetCoreAudio;
using Serilog;
using System;
using System.Diagnostics;
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
        catch (Exception ex)
        {
            Log.Error(ex.Message);
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

            if (player.Playing)
            {
                await player.Stop();
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
        }
    }

    public Task SetBrowserTitle(string title)
    {
        return Task.CompletedTask;
    }

    public Task<bool> OpenUrl(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch(Exception ex)
        {
            Log.Error(ex.Message);
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }

    public Task<string> GetStorageItem(string key)
    {
        return Task.FromResult(string.Empty);
    }

    public Task SetStorageItem(string key, string value)
    {
        return Task.CompletedTask;
    }
}