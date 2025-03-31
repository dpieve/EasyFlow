using EasyFocus.Domain.Entities;
using EasyFocus.Domain.Services;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EasyFocus.Android;

public sealed class PlaySoundAndroid : IPlaySoundService
{
    public Task Play(Sound soundType, int volume)
    {
        Debug.WriteLine("Playing sound on Android");
        return Task.CompletedTask;
    }
}