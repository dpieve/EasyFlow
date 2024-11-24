using EasyFlow.Desktop.Common;
using EasyFlow.Desktop.Features.Focus.AdjustTimers;
using EasyFlow.Desktop.Services;
using EasyFlow.Domain.Entities;
using ReactiveUI;
using SukiUI.Dialogs;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using ReactiveUI.SourceGenerators;
using MediatR;

namespace EasyFlow.Desktop.Features.Focus;

public sealed partial class FocusViewModel : ActivatableSideMenuViewModelBase, IScreen
{
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
    }

    public RoutingState Router { get; } = new();

    public override void HandleActivation(CompositeDisposable d)
    {
        var isStarting = Router.NavigationStack.Count == 0;

        if (isStarting)
        {
            Observable
                .StartAsync(GetSettings)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Select(_ => System.Reactive.Unit.Default)
                .InvokeCommand(NavigateToAdjustTimersCommand);
        }

        Trace.TraceInformation("OnActivated FocusViewModel");

    }

    public override void HandleDeactivation()
    {
        _toastService.DismissAll();
        Trace.TraceInformation("OnDeactivated FocusViewModel");
    }

    [ReactiveCommand]
    private async Task NavigateToAdjustTimers()
    {
        var settings = await GetSettings();
        await Router.Navigate.Execute(new AdjustTimersViewModel(settings, _mediator, _languageService, _toastService, _dialog, _notificationService, this));
    }

    private async Task<GeneralSettings> GetSettings()
    {
        var result = await _mediator.Send(new Application.Settings.Get.Query());
        return result.Value;
    }
}