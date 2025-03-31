using EasyFocus.Domain.Entities;
using EasyFocus.Domain.Services;
using NetCoreAudio;
using System;
using System.Threading.Tasks;

namespace EasyFocus.Services.Desktop;

public sealed class PlaySoundDesktop : IPlaySoundService
{
    public PlaySoundDesktop()
    {
    }

    public async Task Play(Sound type, int volume)
    {
        try
        {
            var player = new Player();
            await player.SetVolume((byte)volume);

            var fileName = $"Assets/{type.GetFileName()}";

            if (player.Playing)
            {
                return;
            }

            await player.Play(fileName);

            await Task.Delay(3000);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}