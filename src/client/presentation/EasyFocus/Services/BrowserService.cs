using EasyFocus.Domain.Services;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;

namespace EasyFocus.Services;

public partial class BrowserTitleApi
{
    [JSImport("setBrowserTitle", "BrowserTitleApi")]
    public static partial void SetBrowserTitle(string title);
}

public sealed class BrowserService : IBrowserService
{
    private bool _isInitialized = false;

    public bool OpenUrl(string url)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Process.Start("xdg-open", url);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Process.Start("open", url);
        }
        else
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