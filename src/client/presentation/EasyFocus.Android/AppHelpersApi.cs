using EasyFocus.Domain.Services;
using System.Threading.Tasks;

namespace EasyFocus.Android;

public class AppHelpersApi : IAppHelpersApi
{
    public AppHelpersApi()
    {
    }

    public Task LogValue(string message)
    {
        return Task.CompletedTask;
    }

    public Task ShowNotification(string title, string message)
    {
        return Task.CompletedTask;
    }

    public Task PlayAudio(string audioFileName, int volume)
    {
        return Task.CompletedTask;
    }

    public Task SetBrowserTitle(string title)
    {
        return Task.CompletedTask;
    }

    public Task<bool> OpenUrl(string url)
    {
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