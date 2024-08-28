using EasyFlow.Domain.Entities;
using EasyFlow.Domain.Services;
using EasyFlow.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using NAudio.Wave;
using System.Diagnostics;

namespace EasyFlow.Infrastructure.Services;
public sealed class PlaySoundService : IPlaySoundService
{
    
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public PlaySoundService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<bool> Play(SoundType type)
    {
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var settings = await context.GeneralSettings.FirstAsync();

            if (settings is null)
            {
                return false;
            }

            var breakSounds = settings.IsBreakSoundEnabled;

            if (type == SoundType.Break && !breakSounds)
            {
                return false;
            }

            var workSounds = settings.IsWorkSoundEnabled;

            if (type == SoundType.Work && !workSounds)
            {
                return false;
            }

            var volume = settings.SoundVolume;
            if (volume == 0)
            {
                return false;
            }

            var assets = "Assets";
            var fileName = GetFileName(type);
            var basePath = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(basePath, assets, fileName);

            WaveOutEvent outputDevice = new();
            AudioFileReader audioFile = new(filePath);

            outputDevice.Init(audioFile);
            outputDevice.Volume = volume / 100.0f;
            outputDevice.Play();
        }
        catch(Exception ex)
        {
            Debug.WriteLine(ex);
            return false;
        }
        
        return true;
    }

    private static string GetFileName(SoundType type) => type switch
    {
        SoundType.Break => "started_break.mp3",
        SoundType.Work => "started_work.mp3",
        _ => throw new System.NotImplementedException(),
    };
}
