using EasyFlow.Data;
using NAudio.Wave;
using System;
using System.IO;
using System.Linq;

namespace EasyFlow.Services;

public enum SoundType
{
    Break,
    Work,
}

public interface IPlaySoundService
{
    public void Play(SoundType type);
}
public sealed class PlaySoundService : IPlaySoundService
{
    public void Play(SoundType type)
    {
        using var context = new AppDbContext();
        var settings = context.GeneralSettings.FirstOrDefault();

        if (settings is null)
        {
            return;
        }

        var breakSounds = settings.IsBreakSoundEnabled;

        if (type == SoundType.Break && !breakSounds)
        {
            return;
        }

        var workSounds = settings.IsWorkSoundEnabled;

        if (type == SoundType.Work && !workSounds)
        {
            return;
        }

        var assets = "Assets";
        var fileName = GetFileName(type);
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assets, fileName);
        
        WaveOutEvent outputDevice = new();
        AudioFileReader audioFile = new(filePath);

        outputDevice.Init(audioFile);
        outputDevice.Play();
    }

    private static string GetFileName(SoundType type) => type switch
    {
        SoundType.Break => "started_break.mp3",
        SoundType.Work => "started_work.mp3",
        _ => throw new System.NotImplementedException(),
    };
}
