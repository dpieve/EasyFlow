using Avalonia.Platform;
using EasyFlow.Domain.Entities;
using EasyFlow.Domain.Services;
using NAudio.Wave;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace EasyFlow.Windows;

internal sealed class PlaySoundDesktop : IPlaySoundService
{
    private readonly ConcurrentDictionary<Sound, CachedSound> _sounds = new();

    public PlaySoundDesktop()
    {
    }

    public async Task Play(Sound type, int volume)
    {
        await Task.Run(() =>
        {
            try
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
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        });
    }
}

internal sealed class CachedSound
{
    public byte[] SoundData { get; }

    public CachedSound(Sound type)
    {
        Uri uri = new($"avares://EasyFlow/Assets/{type.GetFileName()}");
        using Stream assetStream = AssetLoader.Open(uri);
        using MemoryStream memoryStream = new();
        assetStream.CopyTo(memoryStream);
        memoryStream.Seek(0, 0);
        SoundData = memoryStream.ToArray();
    }
}