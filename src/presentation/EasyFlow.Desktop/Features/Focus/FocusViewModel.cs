using EasyFlow.Desktop.Common;
using EasyFlow.Desktop.Services;
using EasyFlow.Domain.Entities;
using MediatR;
using SukiUI.Dialogs;
using System.Threading.Tasks;

namespace EasyFlow.Desktop.Features.Focus;

public sealed partial class FocusViewModel : PageViewModelBase
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

    //protected override void OnActivated()
    //{
    //    if (CurrentRoute is null)
    //    {
    //        Observable
    //        .StartAsync(GetSettings)
    //        .Select(settings => new AdjustTimersViewModel(settings, this, _mediator, _languageService, _toastService, _dialog, _notificationService))
    //        .Subscribe(startVm =>
    //        {
    //            Router.NavigateToAndReset(startVm);
    //            CurrentRoute?.Activate();
    //        });
    //    }
    //    else
    //    {
    //        CurrentRoute?.Activate();
    //    }

    //    Trace.TraceInformation("OnActivated FocusViewModel");
    //}

    //protected override void OnDeactivated()
    //{
    //    CurrentRoute?.Deactivate();

    //    _toastService.DismissAll();

    //    Trace.TraceInformation("OnDeactivated FocusViewModel");
    //}

    private async Task<GeneralSettings> GetSettings()
    {
        var result = await _mediator.Send(new Application.Settings.Get.Query());
        return result.Value;
    }
}