namespace EasyFlow.Domain.Entities;

public enum Sound
{
    None,
    Audio1,
    Audio2,
}

public static class SoundExtensions
{
    public static string GetFileName(this Sound type) => type switch
    {
        Sound.Audio1 => "aud1.wav",
        Sound.Audio2 => "aud2.wav",
        _ => throw new NotImplementedException(),
    };
}