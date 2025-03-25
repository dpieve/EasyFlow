using EasyFlow.Domain.Services;
using System;
using System.Diagnostics;

namespace EasyFlow.Linux;

public sealed class CustomVirtualKeyboard : ICustomVirtualKeyboard
{
    public void Close()
    {
        try
        {
            Process.Start("pkill", "matchbox-keyboard");
        }
        catch (Exception ex)
        {
            Debug.Write(ex.Message);
        }
    }

    public void Open()
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "matchbox-keyboard",
                Arguments = "-geometry 800x300+0+500",
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Debug.Write(ex.Message);
        }
    }
}