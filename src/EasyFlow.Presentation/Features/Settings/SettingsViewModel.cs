using EasyFlow.Presentation.Common;
using EasyFlow.Presentation.Data;
using EasyFlow.Presentation.Features.Settings.General;
using EasyFlow.Presentation.Features.Settings.Tags;
using EasyFlow.Presentation.Services;
using System.Diagnostics;

namespace EasyFlow.Presentation.Features.Settings;

public sealed partial class SettingsViewModel : PageViewModelBase
{
    private readonly IGeneralSettingsService _generalSettingsService;
    private readonly ITagService _tagService;

    public SettingsViewModel(
        IGeneralSettingsService settingsService, 
        ITagService tagService,
        IDatabaseManager databaseMigrator)
        : base("Settings", Material.Icons.MaterialIconKind.Cog, (int)PageOrder.Settings)
    {
        _generalSettingsService = settingsService;
        _tagService = tagService;
        
        Tags = new(settingsService, _tagService);
        GeneralSettings = new(_generalSettingsService, databaseMigrator);
    }
    public TagsViewModel Tags { get; }
    public GeneralSettingsViewModel GeneralSettings { get; }

    protected override void OnActivated()
    {
        Tags.Activate();
        GeneralSettings.Activate();

        Debug.WriteLine("Activated SettingsViewModel");
    }

    protected override void OnDeactivated()
    {
        Tags.Deactivate();
        GeneralSettings.Deactivate();

        Debug.WriteLine("Deactivated SettingsViewModel");
    }
}