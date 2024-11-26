using EasyFlow.Domain.Entities;

namespace EasyFlow.Domain.Services;

public interface IPlaySoundService
{
    public Task<bool> Play(SoundType type, int volume);
}