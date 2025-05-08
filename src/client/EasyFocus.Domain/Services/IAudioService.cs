using EasyFocus.Domain.Entities;

namespace EasyFocus.Domain.Services;

public interface IAudioService
{
    public Task Play(Sound type, int volume);
}