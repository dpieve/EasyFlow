using CommunityToolkit.Mvvm.Input;
using EasyFlow.Presentation.Features.Restart;
using ReactiveUI;
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
                Debug.WriteLine("mainModule is null. Failed to restart");
                return;
            }

            string exePath = mainModule.FileName;
            Process.Start(exePath);

            Process.GetCurrentProcess().Kill();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }
}