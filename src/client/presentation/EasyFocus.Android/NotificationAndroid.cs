using EasyFocus.Domain.Services;
using Serilog;
using System.Threading.Tasks;

namespace EasyFocus.Android;

public sealed class NotificationAndroid : INotificationService
{
    public Task ShowNotification(string title, string message)
    {
        Log.Debug("Showing notification on Android");
        return Task.CompletedTask;
    }
}