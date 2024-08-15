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

    private GeneralSettings? _generalSettings;

    [ObservableProperty]
    private bool _isWorkSoundEnabled = false;

    [ObservableProperty]
    private bool _isBreakSoundEnabled = false;

    [ObservableProperty]
    private bool _isNotificationEnabled = false;

    public GeneralSettingsViewModel(IGeneralSettingsService generalSettingsService)
    {
        _generalSettingsService = generalSettingsService;

        this.WhenAnyValue(
                vm => vm.IsWorkSoundEnabled, 
                vm => vm.IsBreakSoundEnabled, 
                vm => vm.IsNotificationEnabled)
            .Skip(4)
            .Select(_ => Unit.Default)
            .InvokeCommand(PersistSettingsCommand);
    }

    public void Activate()
    {
        Debug.WriteLine("Activated GeneralSettingsViewModel");

        var result = _generalSettingsService.Get();
        if (result.Error is not null)
        {
            SukiHost.ShowToast("Failed to load", "Failed to load the settings", SukiUI.Enums.NotificationType.Error);
            return;
        }

        _generalSettings = result.Value!;

        IsWorkSoundEnabled = _generalSettings.IsWorkSoundEnabled;
        IsBreakSoundEnabled = _generalSettings.IsBreakSoundEnabled;
        IsNotificationEnabled = _generalSettings.IsNotificationEnabled;
    }

    public void Deactivate()
    {
        Debug.WriteLine("Deactivated GeneralSettingsViewModel");
    }

    [RelayCommand]
    private async Task PersistSettings()
    {
        if (_generalSettings is null)
        {
            return;
        }

        _generalSettings.IsWorkSoundEnabled = IsWorkSoundEnabled;
        _generalSettings.IsBreakSoundEnabled = IsBreakSoundEnabled;
        _generalSettings.IsNotificationEnabled = IsNotificationEnabled;

        var result = await _generalSettingsService.UpdateAsync(_generalSettings);
        if (result.Error is not null)
        {
            await SukiHost.ShowToast("Failed to update", "Failed to update the settings", SukiUI.Enums.NotificationType.Error);
        }
        else
        {
            Debug.WriteLine("Persisted settings");
        }
    }
}
