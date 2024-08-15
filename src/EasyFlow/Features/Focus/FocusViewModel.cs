using CommunityToolkit.Mvvm.ComponentModel;
using EasyFlow.Common;
using EasyFlow.Features.Focus.AdjustTimers;
using EasyFlow.Features.Focus.RunningTimer;
using EasyFlow.Services;
using SimpleRouter;
using System;
using System.Diagnostics;

namespace EasyFlow.Features.Focus;

public sealed partial class FocusViewModel : PageViewModelBase, IRouterHost
{
    [ObservableProperty]
    private IRoute? _currentRoute;
    private readonly IGeneralSettingsService _settingsService;
    private readonly ITagService _tagService;

    public FocusViewModel(
        IGeneralSettingsService settingsService,
        ITagService tagService)
        : base("Focus", Material.Icons.MaterialIconKind.Timer, (int)PageOrder.Focus)
    {
        _settingsService = settingsService;
        _tagService = tagService;

        Router = new Router(new RouteFactory(CreateRoutes));
        Router.OnRouteChanged += OnRouteChanged;
        Router.NavigateToAndReset(new AdjustTimersViewModel(this, _tagService));
    }

    public IRouter Router { get; }

    protected override void OnActivated()
    {
        CurrentRoute?.Activate();

        Debug.WriteLine("Activated Focus");
    }

    protected override void OnDeactivated()
    {
        CurrentRoute?.Deactivate();

        Debug.WriteLine("Deactivated Focus");
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

    private IRoute? CreateRoutes(Type routeType, object[] parameters)
    {
        return routeType.Name switch
        {
            nameof(AdjustTimersViewModel) => new AdjustTimersViewModel((FocusViewModel)parameters[0], tagService: _tagService),
            nameof(RunningTimerViewModel) => new RunningTimerViewModel((FocusViewModel)parameters[0], (FocusSettings)parameters[1]),
            _ => null,
        };
    }
}