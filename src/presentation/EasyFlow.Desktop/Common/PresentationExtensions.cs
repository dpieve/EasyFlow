using EasyFlow.Desktop.Features.Settings;
using EasyFlow.Desktop.Services;
using EasyFlow.Desktop.Features.Dashboard;
using EasyFlow.Desktop.Features.Focus;
using Microsoft.Extensions.DependencyInjection;
using SukiUI.Dialogs;
using SukiUI.Toasts;

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


        // Pages
        services
            .AddTransient(typeof(PageViewModelBase), typeof(SettingsViewModel))
            .AddTransient(typeof(PageViewModelBase), typeof(DashboardViewModel))
            .AddTransient(typeof(PageViewModelBase), typeof(FocusViewModel));

        services.AddSingleton<MainViewModel>();

        return services;
    }
}