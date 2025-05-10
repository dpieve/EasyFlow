using EasyFocus.Domain.Services;
using System.Threading.Tasks;

namespace EasyFocus.Resources.Mockups;

public class NotificationServiceMockup : INotificationService
{
    public Task Show(string title, string message)
    {
        return Task.CompletedTask;
    }
}