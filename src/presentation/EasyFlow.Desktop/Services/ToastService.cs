using Avalonia.Controls.Notifications;
using SukiUI.Toasts;
using System;

namespace EasyFlow.Desktop.Services;

public interface IToastService
{
    ISukiToastManager ToastManager { get; }

    void Display(string title, string content, NotificationType type);

    void DismissAll();
}

public sealed class ToastService : IToastService
{
    public ToastService(ISukiToastManager toastManager)
    {
        ToastManager = toastManager;
        ToastManager.EnsureMaximum(3);
    }

    public ISukiToastManager ToastManager { get; set; }

    public void Display(string title, string content, NotificationType type)
    {
        ToastManager.CreateToast()
               .WithTitle(title)
               .WithContent(content)
               .OfType(type)
               .Dismiss().ByClicking()
               .Dismiss().After(TimeSpan.FromSeconds(3))
               .Queue();
    }

    public void DismissAll()
    {
        ToastManager.DismissAll();
    }
}
