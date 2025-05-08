namespace EasyFocus.Domain.Services;

public interface INotificationService
{
    public Task Show(string title, string message);
}