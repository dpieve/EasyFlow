namespace EasyFocus.Domain.Services;

public interface IBrowserService
{
    public Task UpdateTitleAsync(int secondsLeft, bool started);

    public Task<bool> OpenUrlAsync(string url);
}