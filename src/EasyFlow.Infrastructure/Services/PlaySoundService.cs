using EasyFlow.Domain.Entities;
using EasyFlow.Domain.Services;
using Microsoft.Extensions.Logging;
using NAudio.Wave;

namespace EasyFlow.Infrastructure.Services;

public sealed class PlaySoundService : IPlaySoundService
{
    private readonly ILogger<PlaySoundService> _logger;

    public PlaySoundService(ILogger<PlaySoundService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> Play(SoundType type, int volume)
    {
        try
        {
            await Task.Run(() =>
            {
                var assets = "Assets";
                var fileName = GetFileName(type);
                var basePath = Directory.GetCurrentDirectory();
                var filePath = Path.Combine(basePath, assets, fileName);

                WaveOutEvent outputDevice = new();
                AudioFileReader audioFile = new(filePath);

                outputDevice.Init(audioFile);
                outputDevice.Volume = volume / 100.0f;
                outputDevice.Play();
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to play sound");
            return false;
        }

        return true;
    }

    private static string GetFileName(SoundType type) => type switch
    {
        SoundType.Break => "started_break.mp3",
        SoundType.Work => "started_work.mp3",
        _ => throw new NotImplementedException(),
    };
}