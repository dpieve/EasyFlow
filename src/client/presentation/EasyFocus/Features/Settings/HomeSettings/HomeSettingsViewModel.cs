using EasyFocus.Common;
using ReactiveUI.SourceGenerators;
using Serilog;

namespace EasyFocus.Features.Settings.HomeSettings;

public sealed partial class HomeSettingsViewModel : ViewModelBase
{
    [Reactive] private string _errorMessage = string.Empty;

    public HomeSettingsViewModel()
    {
    }

    public bool CanOpenReport => true; // OperatingSystem.IsWindows() || OperatingSystem.IsLinux() || OperatingSystem.IsMacOS();

    [ReactiveCommand]
    private void OnFocusTime()
    {
        Log.Debug("On Focus Time");
        Clean();
    }

    [ReactiveCommand]
    private void OnNotifications()
    {
        Log.Debug("On Notifications Time");
        Clean();
    }

    [ReactiveCommand]
    private bool OnReport()
    {
        Log.Debug("On Report Time");

        if (!CanOpenReport)
        {
            ErrorMessage = "Report only supported on Desktop";
            return false;
        }

        Clean();
        return CanOpenReport;
    }

    [ReactiveCommand]
    private void OnTags()
    {
        Log.Debug("On Tags");
        Clean();
    }

    [ReactiveCommand]
    private void OnBackground()
    {
        Log.Debug("On Background");
        Clean();
    }

    private void Clean()
    {
        ErrorMessage = string.Empty;
    }
}