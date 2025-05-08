using EasyFocus.Domain.Services;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace EasyFocus.Browser;

[SupportedOSPlatform("browser")]
public class AppHelpersApi : IAppHelpersApi
{
    private bool _started = false;

    public AppHelpersApi()
    {
    }

    public async Task LogValue(string message)
    {
        await Start();
        AppHelpersInterop.LogValue(message);
    }

    public async Task ShowNotification(string title, string message)
    {
        await Start();
        AppHelpersInterop.ShowNotification(title, message);
    }

    public async Task PlayAudio(string audioFileName, int volume)
    {
        await Start();
        AppHelpersInterop.PlayAudio(audioFileName, volume);
    }

    public async Task SetBrowserTitle(string title)
    {
        await Start();
        AppHelpersInterop.SetBrowserTitle(title);
    }

    public async Task<bool> OpenUrl(string url)
    {
        await Start();
        AppHelpersInterop.OpenUrl(url);
        return true;
    }

    public async Task<string> GetStorageItem(string key)
    {
        await Start();
        return AppHelpersInterop.GetStorageItem(key) ?? string.Empty;
    }

    public async Task SetStorageItem(string key, string value)
    {
        await Start();
        AppHelpersInterop.SetStorageItem(key, value);
    }

    private async Task Start()
    {
        if (_started)
        {
            return;
        }
        await JSHost.ImportAsync("AppHelpersInterop", "/appHelpers.js");
        _started = true;
    }
}