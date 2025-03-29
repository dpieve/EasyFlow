using EasyFlow.Domain.Entities;

namespace EasyFlow.Domain.Services;

public interface IPlaySoundService
{
    public Task Play(Sound type, int volume);
}
