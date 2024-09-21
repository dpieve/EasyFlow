using CommunityToolkit.Mvvm.ComponentModel;
using EasyFlow.Application.Settings;
using EasyFlow.Domain.Entities;
using EasyFlow.Presentation.Common;
using EasyFlow.Presentation.Features.Focus.AdjustTimers;
using EasyFlow.Presentation.Features.Focus.RunningTimer;
using EasyFlow.Presentation.Services;
using MediatR;
using SimpleRouter;
using SukiUI.Dialogs;
using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace EasyFlow.Presentation.Features.Focus;

public sealed partial class FocusViewModel : PageViewModelBase, IRouterHost
{
    [ObservableProperty]
    private IRoute? _currentRoute;

    private readonly IMediator _mediator;
    private readonly ILanguageService _languageService;
    private readonly IToastService _toastService;
    private readonly ISukiDialogManager _dialog;
    private readonly INotificationService _notificationService;

    public FocusViewModel(
        IMediator mediator,
        ILanguageService languageService,
        IToastService toastService,
        ISukiDialogManager dialog,
        INotificationService notificationService)
        : base(ConstantTranslation.SideMenuFocus, Material.Icons.MaterialIconKind.Timer, (int)PageOrder.Focus)
    {
        _mediator = mediator;
        _languageService = languageService;
        _toastService = toastService;
        _dialog = dialog;
        _notificationService = notificationService;
        
        Router = new Router(new RouteFactory(CreateRoutes));
        Router.OnRouteChanged += OnRouteChanged;
    }

    public IRouter Router { get; }

    protected override void OnActivated()
    {
        if (CurrentRoute is null)
        {
            Observable
            .StartAsync(GetSettings)
            .Select(settings => new AdjustTimersViewModel(settings, this, _mediator, _languageService, _toastService, _dialog, _notificationService))
            .Subscribe(startVm =>
            {
                Router.NavigateToAndReset(startVm);
                CurrentRoute?.Activate();
            });
        }
        else
        {
            CurrentRoute?.Activate();
        }

        Trace.TraceInformation("OnActivated FocusViewModel");
    }

    protected override void OnDeactivated()
    {
        CurrentRoute?.Deactivate();

        _toastService.DismissAll();

        Trace.TraceInformation("OnDeactivated FocusViewModel");
    }

    private async Task<GeneralSettings> GetSettings()
    {
        var result = await _mediator.Send(new GetSettingsQuery());
        return result.Value;
    }

    private void OnRouteChanged(object? sender, RouteChangedEventArgs e)
    {
        var isFirstCall = CurrentRoute is null;

        CurrentRoute?.Deactivate();

        CurrentRoute = e.Next;

        if (!isFirstCall)
        {
            CurrentRoute?.Activate();
        }
    }

    private static IRoute? CreateRoutes(Type routeType, object[] parameters)
    {
        return routeType.Name switch
        {
            nameof(AdjustTimersViewModel) => new AdjustTimersViewModel((GeneralSettings)parameters[0], (FocusViewModel)parameters[1], (IMediator)parameters[2], (ILanguageService)parameters[3], (IToastService)parameters[4], (ISukiDialogManager)parameters[5], (INotificationService)parameters[6]),
            nameof(RunningTimerViewModel) => new RunningTimerViewModel((FocusViewModel)parameters[0], (IMediator)parameters[1], (ILanguageService)parameters[2], (IToastService)parameters[3], (ISukiDialogManager)parameters[4], (INotificationService)parameters[5]),
            _ => null,
        };
    }
}