using EasyFocus.Domain.Services;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EasyFocus.Android;

public sealed class NotificationAndroid : INotificationService
{
    public Task ShowNotification(string title, string message)
    {
        Debug.WriteLine("Showing notification on Android");
        return Task.CompletedTask;
    }
}