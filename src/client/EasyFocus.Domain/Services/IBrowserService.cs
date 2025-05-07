namespace EasyFocus.Domain.Services;

public interface IBrowserService
{
    public Task UpdateTitleAsync(int secondsLeft, bool started);

    public bool OpenUrl(string url);
}