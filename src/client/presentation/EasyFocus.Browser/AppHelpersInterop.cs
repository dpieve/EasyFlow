using System.Runtime.InteropServices.JavaScript;

namespace EasyFocus.Browser;

internal partial class AppHelpersInterop
{
    [JSImport("logValue", "AppHelpersInterop")]
    public static partial void LogValue(string message);

    [JSImport("showNotification", "AppHelpersInterop")]
    public static partial void ShowNotification(string title, string message);

    [JSImport("playAudio", "AppHelpersInterop")]
    public static partial void PlayAudio(string audioFileName, int volume);

    [JSImport("setBrowserTitle", "AppHelpersInterop")]
    public static partial void SetBrowserTitle(string title);

    [JSImport("openUrl", "AppHelpersInterop")]
    public static partial void OpenUrl(string url);

    [JSImport("getStorageItem", "AppHelpersInterop")]
    public static partial string? GetStorageItem(string key);

    [JSImport("setStorageItem", "AppHelpersInterop")]
    public static partial void SetStorageItem(string key, string value);
}