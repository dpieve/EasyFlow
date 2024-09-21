using EasyFlow.Presentation.Common;
using EasyFlow.Presentation.Features.Settings.General;
using EasyFlow.Presentation.Features.Settings.Tags;
using EasyFlow.Presentation.Services;
using MediatR;
using SukiUI.Controls;

namespace EasyFlow.Presentation.Features.Settings;

public sealed partial class SettingsViewModel : PageViewModelBase
{
    private readonly IMediator _mediator;
    private readonly IRestartAppService _restartAppService;

    public SettingsViewModel(IMediator mediator, IRestartAppService restartAppService, ILanguageService languageService)
        : base(ConstantTranslation.SideMenuSettings, Material.Icons.MaterialIconKind.Cog, (int)PageOrder.Settings)
    {
        _mediator = mediator;
        _restartAppService = restartAppService;

        Tags = new TagsViewModel(_mediator, languageService);
        GeneralSettings = new GeneralSettingsViewModel(_mediator, _restartAppService, languageService);
    }

    public TagsViewModel Tags { get; }
    public GeneralSettingsViewModel GeneralSettings { get; }

    protected override void OnActivated()
    {
        Tags.Activate();
        GeneralSettings.Activate();
    }

    protected override void OnDeactivated()
    {
        Tags.Deactivate();
        GeneralSettings.Deactivate();

        //SukiHost.ClearAllToasts();
    }
}