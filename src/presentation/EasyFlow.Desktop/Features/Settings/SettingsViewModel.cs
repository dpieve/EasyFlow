using EasyFlow.Desktop.Services;
using EasyFlow.Desktop.Common;
using EasyFlow.Desktop.Features.Settings.General;
using EasyFlow.Desktop.Features.Settings.Tags;
using EasyFlow.Desktop.Services;
using MediatR;
using SukiUI.Dialogs;

namespace EasyFlow.Desktop.Features.Settings;

public sealed partial class SettingsViewModel : SideMenuViewModelBase
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

    //protected override void OnActivated()
    //{
    //    Tags.Activate();
    //    GeneralSettings.Activate();
    //}

    //protected override void OnDeactivated()
    //{
    //    Tags.Deactivate();
    //    GeneralSettings.Deactivate();

    //    _toastService.DismissAll();
    //}
}