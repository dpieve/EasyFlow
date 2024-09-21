using EasyFlow.Presentation.Features.Dashboard;
using EasyFlow.Presentation.Features.Focus;
using EasyFlow.Presentation.Features.Settings;
using EasyFlow.Presentation.Services;
using Microsoft.Extensions.DependencyInjection;
using SukiUI.Dialogs;
using SukiUI.Toasts;

namespace EasyFlow.Presentation.Common;

public static class PresentationExtensions
{
    public static ServiceCollection AddPresentation(this ServiceCollection services)
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