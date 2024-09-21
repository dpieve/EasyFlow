using CommunityToolkit.Mvvm.ComponentModel;
using EasyFlow.Application.Settings;
using EasyFlow.Domain.Entities;
using EasyFlow.Presentation.Common;
using EasyFlow.Presentation.Features.Focus.AdjustTimers;
using EasyFlow.Presentation.Features.Focus.RunningTimer;
using EasyFlow.Presentation.Services;
using MediatR;
using SimpleRouter;
using SukiUI.Controls;
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

    public FocusViewModel(
        IMediator mediator,
        ILanguageService languageService)
        : base(ConstantTranslation.SideMenuFocus, Material.Icons.MaterialIconKind.Timer, (int)PageOrder.Focus)
    {
        _mediator = mediator;
        _languageService = languageService;

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
            .Select(settings => new AdjustTimersViewModel(settings, this, _mediator, _languageService))
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

        //SukiHost.ClearAllToasts();

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
            nameof(AdjustTimersViewModel) => new AdjustTimersViewModel((GeneralSettings)parameters[0], (FocusViewModel)parameters[1], (IMediator)parameters[2], (ILanguageService)parameters[3]),
            nameof(RunningTimerViewModel) => new RunningTimerViewModel((FocusViewModel)parameters[0], (IMediator)parameters[1], (ILanguageService)parameters[2]),
            _ => null,
        };
    }
}