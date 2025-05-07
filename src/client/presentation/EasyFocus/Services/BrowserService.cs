using EasyFocus.Domain.Services;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;

namespace EasyFocus.Services;

public partial class BrowserTitleApi
{
    [JSImport("setBrowserTitle", "BrowserTitleApi")]
    public static partial void SetBrowserTitle(string title);

    [JSImport("logValue", "BrowserTitleApi")]
    public static partial void LogValue(string message);

    [JSImport("openUrl", "BrowserTitleApi")]
    public static partial void OpenUrl(string url);
}

public sealed class BrowserService : IBrowserService
{
    private bool _isInitialized = false;

    public async Task<bool> OpenUrlAsync(string url)
    {
        try
        {
            if (OperatingSystem.IsBrowser())
            {
                if (!_isInitialized)
                {
                    await JSHost.ImportAsync("BrowserTitleApi", "/TitleJs.js");
                    _isInitialized = true;
                }

                BrowserTitleApi.OpenUrl(url);

                return true;
            }

            if (OperatingSystem.IsWindows())
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });

                return true;
            }

            if (OperatingSystem.IsLinux())
            {
                Process.Start("xdg-open", url);

                return true;
            }

            if (OperatingSystem.IsMacOS())
            {
                Process.Start("open", url);

                return true;
            }
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    public async Task UpdateTitleAsync(int secondsLeft, bool started)
    {
        if (!OperatingSystem.IsBrowser())
        {
            return;
        }

        if (!_isInitialized)
        {
            await JSHost.ImportAsync("BrowserTitleApi", "/TitleJs.js");
            _isInitialized = true;
        }

        var appName = "EasyFocus";

        if (!started || secondsLeft == 0)
        {
            BrowserTitleApi.SetBrowserTitle(appName);
            return;
        }

        string title = $"{secondsLeft / 60}:{secondsLeft % 60:D2} | {appName}";
        BrowserTitleApi.SetBrowserTitle(title);
    }
}