﻿namespace EasyFocus.Domain.Entities;

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
        Sound.Audio1 => "audio1.wav",
        Sound.Audio2 => "audio2.wav",
        _ => throw new NotImplementedException(),
    };
}