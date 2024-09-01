using CommunityToolkit.Mvvm.Input;
using EasyFlow.Presentation.Features.Restart;
using ReactiveUI;
using Serilog;
using SukiUI.Controls;
using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
namespace EasyFlow.Presentation.Services;

public interface IRestartAppService
{
    public void Restart();
}

public sealed partial class RestartAppService : IRestartAppService
{
    private readonly Subject<Unit> _restart = new();

    public RestartAppService()
    {
        _restart
            .ObserveOn(RxApp.MainThreadScheduler)
            .InvokeCommand(RestartAppCommand);
    }

    public void Restart()
    {
        SukiHost.ClearAllToasts();
        Task.Delay(200).Wait();

        SukiHost.ShowDialog(new RestartViewModel(() =>
        {
            _restart.OnNext(Unit.Default);
        }, secondsBeforeRestart: 3)
        , allowBackgroundClose: false);
    }

    [RelayCommand]
    private static void RestartApp()
    {
        try
        {
            var mainModule = Process.GetCurrentProcess().MainModule;
            if (mainModule is null)
            {
                Trace.TraceError("mainModule is null. Failed to restart");
                return;
            }

            string exePath = mainModule.FileName;

            if (OperatingSystem.IsWindows())
            {
                Process.Start("cmd.exe", $"/C \"{exePath}\"");
            }
            else if (OperatingSystem.IsLinux())
            {
                Process.Start("bash", $"-c \"{exePath}\"");
            }
            else
            {
                Trace.TraceError("Unsupported operating system. Failed to restart");
                return;
            }

            Process.GetCurrentProcess().Kill();
        }
        catch (Exception ex)
        {
            Log.Error("Failed to restart app {Error}", ex.Message);
        }
    }
}