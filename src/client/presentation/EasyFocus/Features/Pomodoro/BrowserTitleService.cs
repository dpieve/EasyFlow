using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace EasyFocus.Features.Pomodoro;

public partial class BrowserTitleApi
{
    [JSImport("setBrowserTitle", "BrowserTitleApi")]
    public static partial void SetBrowserTitle(string title);
}

[SupportedOSPlatform("browser")]
public sealed class BrowserTitleService
{
    private bool _isInitialized = false;

    public async Task Update(int secondsLeft, bool started)
    {
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