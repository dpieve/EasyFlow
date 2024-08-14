using EasyFlow.Common;
using EasyFlow.Features.Settings.General;
using EasyFlow.Features.Settings.Tags;
using EasyFlow.Services;
using System.Diagnostics;

namespace EasyFlow.Features.Settings;

public sealed partial class SettingsViewModel : PageViewModelBase
{
    private readonly IGeneralSettingsService _generalSettingsService;

    public SettingsViewModel(IGeneralSettingsService settingsService)
        : base("Settings", Material.Icons.MaterialIconKind.Cog, (int)PageOrder.Settings)
    {
        _generalSettingsService = settingsService;

        Tags = new();
        GeneralSettings = new(_generalSettingsService);
    }

    public GeneralSettingsViewModel GeneralSettings { get; }
    public TagsViewModel Tags { get; }

    protected override void OnActivated()
    {
        Debug.WriteLine("OnActivated - Settings");

        Tags.Activate();
        GeneralSettings.Activate();
    }

    protected override void OnDeactivated()
    {
        Debug.WriteLine("OnDeactivated - Settings");

        Tags.Deactivate();
        GeneralSettings.Deactivate();
    }
}