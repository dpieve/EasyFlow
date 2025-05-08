namespace EasyFocus.Domain.Services;

public interface IAppHelpersApi
{
    public Task LogValue(string message);

    public Task ShowNotification(string title, string message);

    public Task PlayAudio(string audioFileName, int volume);

    public Task SetBrowserTitle(string title);

    public Task<bool> OpenUrl(string url);

    public Task<string> GetStorageItem(string key);

    public Task SetStorageItem(string key, string value);
}