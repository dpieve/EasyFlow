using EasyFocus.Domain.Entities;
using EasyFocus.Domain.Services;
using Serilog;
using System.Threading.Tasks;

namespace EasyFocus.Android;

public sealed class PlaySoundAndroid : IPlaySoundService
{
    public Task Play(Sound soundType, int volume)
    {
        Log.Debug("Playing sound on Android");
        return Task.CompletedTask;
    }
}