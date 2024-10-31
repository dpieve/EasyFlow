using EasyFlow.Desktop.Services;
using EasyFlow.Desktop.Common;
using EasyFlow.Desktop.Features.Settings.General;
using EasyFlow.Desktop.Features.Settings.Tags;
using MediatR;
using SukiUI.Dialogs;
using System.Reactive.Disposables;

namespace EasyFlow.Desktop.Features.Settings;

public sealed partial class SettingsViewModel : ActivatableSideMenuViewModelBase
{
    private readonly IMediator _mediator;
    private readonly IRestartAppService _restartAppService;
    private readonly IToastService _toastService;

    public SettingsViewModel(
        IMediator mediator,
        IRestartAppService restartAppService,
        ILanguageService languageService,
        IToastService toastService,
        ISukiDialogManager dialog)
        : base(ConstantTranslation.SideMenuSettings, Material.Icons.MaterialIconKind.Cog, (int)PageOrder.Settings)
    {
        _mediator = mediator;
        _restartAppService = restartAppService;
        _toastService = toastService;

        Tags = new TagsViewModel(_mediator, languageService, _toastService, dialog);
        GeneralSettings = new GeneralSettingsViewModel(_mediator, _restartAppService, languageService, _toastService, dialog);
    }

    public TagsViewModel Tags { get; }
    public GeneralSettingsViewModel GeneralSettings { get; }

    public override void HandleActivation(CompositeDisposable d)
    {
    }

    public override void HandleDeactivation()
    {
        _toastService.DismissAll();
    }
}