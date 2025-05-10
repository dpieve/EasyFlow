using EasyFocus.Domain.Entities;
using EasyFocus.Domain.Services;
using System.Threading.Tasks;

namespace EasyFocus.Resources.Mockups;

public class AudioServiceMockup : IAudioService
{
    public Task Play(Sound type, int volume)
    {
        return Task.CompletedTask;
    }
}