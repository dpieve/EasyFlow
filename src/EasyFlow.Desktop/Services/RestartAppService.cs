using EasyFlow.Desktop.Features.Restart;
using ReactiveUI;
using Serilog;
using SukiUI.Dialogs;
using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using ReactiveUI.SourceGenerators;
namespace EasyFlow.Desktop.Services;

public interface IRestartAppService
{
    public void Restart();
}

public sealed partial class RestartAppService : IRestartAppService
{
    private readonly Subject<Unit> _restart = new();
    private readonly IToastService _toastService;
    private readonly ISukiDialogManager _dialog;

    public RestartAppService(IToastService toastService, ISukiDialogManager dialog)
    {
        _toastService = toastService;
        _dialog = dialog;
        
        _restart
            .ObserveOn(RxApp.MainThreadScheduler)
            .InvokeCommand(RestartAppCommand);
    }

    public void Restart()
    {
        _toastService.DismissAll();
        Task.Delay(200).Wait();

        _dialog.CreateDialog()
            .WithViewModel(dialog => new RestartViewModel(dialog, () =>
            {
                _restart.OnNext(Unit.Default);
            }, secondsBeforeRestart: 3))
            .TryShow();
    }

    [ReactiveCommand]
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
                var startInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    UseShellExecute = false,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    CreateNoWindow = true 
                };
                 
                Process.Start(startInfo);
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