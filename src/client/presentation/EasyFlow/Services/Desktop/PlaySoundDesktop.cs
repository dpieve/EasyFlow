using EasyFlow.Domain.Entities;
using EasyFlow.Domain.Services;
using NetCoreAudio;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EasyFlow.Services.Desktop;

public sealed class PlaySoundDesktop : IPlaySoundService
{
    private readonly Player _player = new();

    public PlaySoundDesktop()
    {
    }

    public async Task Play(Sound type, int volume)
    {
        await Task.Run(async () =>
        {
            try
            {
                if (_player.Playing)
                {
                    await _player.Stop();
                }

                var fileName = $"Assets/{type.GetFileName()}";

                await _player.SetVolume((byte)volume);

                if (_player.Playing)
                {
                    return;
                }

                await _player.Play(fileName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        });
    }
}