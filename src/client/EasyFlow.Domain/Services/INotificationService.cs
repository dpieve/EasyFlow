namespace EasyFlow.Domain.Services;

public interface INotificationService
{
    public Task ShowNotification(string title, string message);
}