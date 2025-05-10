using EasyFocus.Domain.Services;
using System.Threading.Tasks;

namespace EasyFocus.Resources.Mockups;

public class BrowserServiceMockup : IBrowserService
{
    public Task<bool> OpenUrlAsync(string url)
    {
        return Task.FromResult(true);
    }

    public Task UpdateTitleAsync(int secondsLeft, bool started)
    {
        return Task.CompletedTask;
    }
}