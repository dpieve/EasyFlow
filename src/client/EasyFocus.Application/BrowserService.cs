using EasyFocus.Domain.Services;

namespace EasyFocus.Application;

public sealed class BrowserService : IBrowserService
{
    private readonly IAppHelpersApi _api;

    public BrowserService(IAppHelpersApi api)
    {
        _api = api;
    }

    public async Task<bool> OpenUrlAsync(string url)
    {
        return await _api.OpenUrl(url);
    }

    public async Task UpdateTitleAsync(int secondsLeft, bool started)
    {
        if (!OperatingSystem.IsBrowser())
        {
            return;
        }

        var baseTitle = "EasyFocus";

        if (!started || secondsLeft == 0)
        {
            await _api.SetBrowserTitle(baseTitle);
            return;
        }

        string title = $"{secondsLeft / 60}:{secondsLeft % 60:D2} | {baseTitle}";
        await _api.SetBrowserTitle(title);
    }
}