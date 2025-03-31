using EasyFocus.Domain.Services;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace EasyFocus.Browser;

public partial class NotificationApi
{
    [JSImport("showNotification", "NotificationApi")]
    public static partial void ShowNotification(string title, string message);
}

[SupportedOSPlatform("browser")]
public sealed class NotificationBrowser : INotificationService
{
    private bool _isInitialized = false;

    public async Task ShowNotification(string title, string message)
    {
        if (!_isInitialized)
        {
            await JSHost.ImportAsync("NotificationApi", "/NotificationJs.js");
            _isInitialized = true;
        }

        NotificationApi.ShowNotification(title, message);
    }
}