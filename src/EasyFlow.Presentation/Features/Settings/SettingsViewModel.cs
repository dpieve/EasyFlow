using EasyFlow.Presentation.Common;
using EasyFlow.Presentation.Features.Settings.General;
using EasyFlow.Presentation.Features.Settings.Tags;
using MediatR;

namespace EasyFlow.Presentation.Features.Settings;

public sealed partial class SettingsViewModel : PageViewModelBase
{
    private readonly IMediator _mediator;

    public SettingsViewModel(IMediator mediator)
        : base("Settings", Material.Icons.MaterialIconKind.Cog, (int)PageOrder.Settings)
    {
        _mediator = mediator;

        Tags = new TagsViewModel(_mediator);
        GeneralSettings = new GeneralSettingsViewModel(_mediator);
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
    }
}