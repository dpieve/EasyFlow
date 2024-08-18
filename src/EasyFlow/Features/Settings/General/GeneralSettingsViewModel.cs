using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyFlow.Common;
using EasyFlow.Data;
using EasyFlow.Services;
using ReactiveUI;
using SukiUI.Controls;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace EasyFlow.Features.Settings.General;

public partial class GeneralSettingsViewModel : ViewModelBase
{
    private readonly IGeneralSettingsService _generalSettingsService;
    private readonly IDatabaseManager _databaseMigrator;

    [ObservableProperty]
    private bool _isWorkSoundEnabled;

    [ObservableProperty]
    private bool _isBreakSoundEnabled;

    public GeneralSettingsViewModel(
        IGeneralSettingsService generalSettingsService,
        IDatabaseManager databaseMigrator)
    {
        _generalSettingsService = generalSettingsService;
        _databaseMigrator = databaseMigrator;
        
        var settings = LoadSettings();
        IsWorkSoundEnabled = settings.IsWorkSoundEnabled;
        IsBreakSoundEnabled = settings.IsBreakSoundEnabled;

        this.WhenAnyValue(
                vm => vm.IsWorkSoundEnabled,
                vm => vm.IsBreakSoundEnabled)
            .Skip(1)
            .Select(_ => Unit.Default)
            .InvokeCommand(PersistSettingsCommand);
    }

    public void Activate()
    {
        Debug.WriteLine("Activated GeneralSettingsViewModel");
    }

    public void Deactivate()
    {
        Debug.WriteLine("Deactivated GeneralSettingsViewModel");
    }

    [RelayCommand]
    private async Task PersistSettings()
    {
        var result = _generalSettingsService.Get();

        if (result.Error is not null)
        {
            await SukiHost.ShowToast("Failed to load", "Failed to load the settings", SukiUI.Enums.NotificationType.Error);
            return;
        }

        var settings = result.Value!;

        settings.IsWorkSoundEnabled = IsWorkSoundEnabled;
        settings.IsBreakSoundEnabled = IsBreakSoundEnabled;

        var resultUpdate = await _generalSettingsService.UpdateAsync(settings);
        if (resultUpdate.Error is not null)
        {
            await SukiHost.ShowToast("Failed to update", "Failed to update the settings", SukiUI.Enums.NotificationType.Error);
            return;
        }
        
        Debug.WriteLine("Persisted settings");
    }

    [RelayCommand]
    private void ClearData()
    {
        SukiHost.ShowDialog(new ClearDataViewModel(_databaseMigrator, () =>
        {
            SukiHost.ShowToast("Data cleared", "All data has been cleared. Restart the software to complete the deletion.", SukiUI.Enums.NotificationType.Success);
        }),
        allowBackgroundClose: false);
    }

    private GeneralSettings LoadSettings()
    {
        var result = _generalSettingsService.Get();

        if (result.Error is not null)
        {
            SukiHost.ShowToast("Failed to load", "Failed to load the settings", SukiUI.Enums.NotificationType.Error);
            return new();
        }

        var settings = result.Value!;
        return settings;
    }
}