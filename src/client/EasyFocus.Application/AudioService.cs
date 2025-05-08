using EasyFocus.Domain.Entities;
using EasyFocus.Domain.Services;

namespace EasyFocus.Application;

public sealed class AudioService : IAudioService
{
    private readonly IAppHelpersApi _api;

    public AudioService(IAppHelpersApi api)
    {
        _api = api;
    }

    public async Task Play(Sound soundType, int volume)
    {
        await _api.PlayAudio(soundType.GetFileName(), volume);
    }
}