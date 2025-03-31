using EasyFocus.Domain.Entities;

namespace EasyFocus.Domain.Services;

public interface IPlaySoundService
{
    public Task Play(Sound type, int volume);
}