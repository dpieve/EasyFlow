using EasyFocus.Common;
using ReactiveUI.SourceGenerators;
using System.Diagnostics;

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
        Debug.WriteLine("On Focus Time");
        Clean();
    }

    [ReactiveCommand]
    private void OnNotifications()
    {
        Debug.WriteLine("On Notifications Time");
        Clean();
    }

    [ReactiveCommand]
    private bool OnReport()
    {
        Debug.WriteLine("On Report Time");

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
        Debug.WriteLine("On Tags");
        Clean();
    }

    [ReactiveCommand]
    private void OnBackground()
    {
        Debug.WriteLine("On Background");
        Clean();
    }

    private void Clean()
    {
        ErrorMessage = string.Empty;
    }
}