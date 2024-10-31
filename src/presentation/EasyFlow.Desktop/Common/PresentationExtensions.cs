using EasyFlow.Desktop.Features.Settings;
using EasyFlow.Desktop.Services;
using EasyFlow.Desktop.Features.Dashboard;
using EasyFlow.Desktop.Features.Focus;
using Microsoft.Extensions.DependencyInjection;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using ReactiveUI;
using Splat;
using EasyFlow.Desktop.Features.Focus.AdjustTimers;
using EasyFlow.Desktop.Features.Focus.RunningTimer;

namespace EasyFlow.Desktop.Common;

public static class PresentationExtensions
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        // Services
        services.AddSingleton<ILanguageService, LanguageService>();

        ISukiToastManager toastManager = new SukiToastManager();
        IToastService toastService = new ToastService(toastManager);
        services.AddSingleton(toastService);

        ISukiDialogManager dialog = new SukiDialogManager();
        services.AddSingleton(dialog);

        IRestartAppService restartAppService = new RestartAppService(toastService, dialog);
        services.AddSingleton(restartAppService);

        services.AddSingleton<MainViewModel>();
        
        // Side Menu
        services
            .AddTransient(typeof(SideMenuViewModelBase), typeof(SettingsViewModel))
            .AddTransient(typeof(SideMenuViewModelBase), typeof(DashboardViewModel))
            .AddTransient(typeof(SideMenuViewModelBase), typeof(FocusViewModel));

        // Pages
        Locator.CurrentMutable.Register(() => new AdjustTimersView(), typeof(IViewFor<AdjustTimersViewModel>));
        Locator.CurrentMutable.Register(() => new RunningTimerView(), typeof(IViewFor<RunningTimerViewModel>));

        return services;
    }
}