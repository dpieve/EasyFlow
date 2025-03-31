using EasyFocus.Domain.Entities;
using EasyFocus.Domain.Services;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace EasyFocus.Browser;

public partial class PlaySoundApi
{
    [JSImport("playAudio", "PlaySoundApi")]
    public static partial int Play(string audioFileName, int volume);

    [JSImport("logValue", "PlaySoundApi")]
    public static partial void LogValue(string message);

    [JSImport("showNotification", "PlaySoundApi")]
    public static partial void ShowNotification(string title, string message);
}

[SupportedOSPlatform("browser")]
public sealed class PlaySoundBrowser : IPlaySoundService
{
    private bool _isInitialized = false;

    public async Task Play(Sound soundType, int volume)
    {
        if (!_isInitialized)
        {
            await JSHost.ImportAsync("PlaySoundApi", "/PlayJs.js");
            _isInitialized = true;
        }
        PlaySoundApi.LogValue("Playing the audio");

        PlaySoundApi.Play(soundType.GetFileName(), volume);
    }
}