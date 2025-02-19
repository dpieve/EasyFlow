using Avalonia.Platform;
using EasyFlow.Domain.Entities;
using EasyFlow.Domain.Services;
using Microsoft.Extensions.Logging;
using NAudio.Wave;
using System.Collections.Concurrent;

namespace EasyFlow.Desktop.Services;

internal sealed class PlaySoundService : IPlaySoundService
{
    private readonly ILogger<PlaySoundService> _logger;
    private readonly ConcurrentDictionary<SoundType, CachedSound> _sounds = new();

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
                if (!_sounds.TryGetValue(type, out var cachedSound))
                {
                    cachedSound = new CachedSound(type);
                    _sounds.TryAdd(type, cachedSound);
                }
                WaveOutEvent outputDevice = new() { Volume = volume / 100.0f };
                MemoryStream stream = new(cachedSound.SoundData);
                outputDevice.Init(new WaveFileReader(stream));
                outputDevice.Play();
                outputDevice.PlaybackStopped += (sender, args) =>
                {
                    outputDevice.Dispose();
                    stream.Dispose();
                };
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to play sound");
            return false;
        }

        return true;
    }
}

internal sealed class CachedSound
{
    public byte[] SoundData { get; }

    public CachedSound(SoundType type)
    {
        Uri uri = new($"avares://EasyFlow.Desktop/Assets/{GetFileName(type)}");
        using Stream assetStream = AssetLoader.Open(uri);
        using MemoryStream memoryStream = new();
        assetStream.CopyTo(memoryStream);
        memoryStream.Seek(0, 0);
        SoundData = memoryStream.ToArray();
    }

    private static string GetFileName(SoundType type) => type switch
    {
        SoundType.Break => "started_break.wav",
        SoundType.Work => "started_work.wav",
        _ => throw new NotImplementedException(),
    };
}