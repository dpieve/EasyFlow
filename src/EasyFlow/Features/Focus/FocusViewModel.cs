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
    private readonly IGeneralSettingsService _settingsService;
    private readonly ITagService _tagService;
    private readonly IPlaySoundService _playSoundService;
    private readonly ISessionService _sessionService;

    [ObservableProperty]
    private IRoute? _currentRoute;

    public FocusViewModel(
        IGeneralSettingsService settingsService,
        ITagService tagService,
        IPlaySoundService playSoundService,
        ISessionService sessionService)
        : base("Focus", Material.Icons.MaterialIconKind.Timer, (int)PageOrder.Focus)
    {
        _settingsService = settingsService;
        _tagService = tagService;
        _playSoundService = playSoundService;
        _sessionService = sessionService;

        Router = new Router(new RouteFactory(CreateRoutes));
        Router.OnRouteChanged += OnRouteChanged;
        Router.NavigateToAndReset(new AdjustTimersViewModel(this, _tagService, _settingsService));
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
            nameof(AdjustTimersViewModel) => new AdjustTimersViewModel((FocusViewModel)parameters[0], tagService: _tagService, generalSettingsService: _settingsService),
            nameof(RunningTimerViewModel) => new RunningTimerViewModel((FocusViewModel)parameters[0], tagService: _tagService, generalSettingsService: _settingsService, playSoundService: _playSoundService, sessionService: _sessionService),
            _ => null,
        };
    }
}