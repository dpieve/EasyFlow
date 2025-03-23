using EasyFlow.Domain.Entities;
using EasyFlow.Domain.Services;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EasyFlow.Android;

public sealed class PlaySoundAndroid : IPlaySoundService
{
    public Task Play(Sound soundType, int volume)
    {
        Debug.WriteLine("Playing sound on Android");
        return Task.CompletedTask;
    }
}