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
                    var assets = "Assets";
                    var fileName = GetFileName(type);
                    var basePath = Directory.GetCurrentDirectory();
                    var filePath = Path.Combine(basePath, assets, fileName);
                    cachedSound = new CachedSound(filePath);
                    _sounds.TryAdd(type, cachedSound);
                }

                WaveOutEvent outputDevice = new() { Volume = volume / 100.0f };

                outputDevice.Init(new CachedSoundSampleProvider(cachedSound));
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

internal sealed class CachedSound
{
    public float[] AudioData { get; private set; }
    public WaveFormat WaveFormat { get; private set; }

    public CachedSound(string audioFileName)
    {
        using var audioFileReader = new AudioFileReader(audioFileName);
        WaveFormat = audioFileReader.WaveFormat;
        var wholeFile = new List<float>((int)(audioFileReader.Length / 4));
        var readBuffer = new float[audioFileReader.WaveFormat.SampleRate * audioFileReader.WaveFormat.Channels];
        int samplesRead;
        while ((samplesRead = audioFileReader.Read(readBuffer, 0, readBuffer.Length)) > 0)
        {
            wholeFile.AddRange(readBuffer.Take(samplesRead));
        }
        AudioData = [.. wholeFile];
    }
}

internal sealed class CachedSoundSampleProvider : ISampleProvider
{
    private readonly CachedSound _cachedSound;
    private int _position;

    public CachedSoundSampleProvider(CachedSound cachedSound)
    {
        _cachedSound = cachedSound;
    }

    public int Read(float[] buffer, int offset, int count)
    {
        var availableSamples = _cachedSound.AudioData.Length - _position;
        var samplesToCopy = Math.Min(availableSamples, count);
        Memory<float> src = new(_cachedSound.AudioData, _position, samplesToCopy);
        Memory<float> dst = new(buffer, offset, count);
        src.TryCopyTo(dst);
        _position += samplesToCopy;
        return samplesToCopy;
    }

    public WaveFormat WaveFormat { get { return _cachedSound.WaveFormat; } }
}