using NAudio.Wave;
using System.IO;

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
        var fileName = GetFileName(type);

        string workingDirectory = Directory.GetCurrentDirectory();
        string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.Parent.FullName;
        string assetsDirectory = Path.Combine(projectDirectory, "EasyFlow\\Assets");
        string filePath = Path.Combine(assetsDirectory, fileName);

        WaveOutEvent outputDevice = new WaveOutEvent();
        AudioFileReader audioFile = new AudioFileReader(filePath);

        outputDevice.Init(audioFile);
        outputDevice.Play();
    }

    private static string GetFileName(SoundType type) => type switch
    {
        SoundType.Break => "break_started.mp3",
        SoundType.Work => "work_started.mp3",
        _ => throw new System.NotImplementedException(),
    };
}
