using CommunityToolkit.Mvvm.Input;
using EasyFlow.Presentation.Common;
using SukiUI.Controls;
using System;

namespace EasyFlow.Presentation.Features.Settings.General;
public sealed partial class ClearDataViewModel : ViewModelBase
{
    private readonly Action? _onOk;

    public ClearDataViewModel( Action? onOk = null)
    {
        _onOk = onOk;
    }

    [RelayCommand]
    private void Ok()
    {
        try 
        {
            //var result = _databaseMigrator.Reset();
            //if (result && _onOk is not null)
            //{
                _onOk();
            //}
        }
        catch(Exception e)
        {
            SukiHost.ShowToast("Failed to delete", "Failed to delete the database.", SukiUI.Enums.NotificationType.Error);
        }
        
        Close();
    }

    [RelayCommand]
    private void Cancel()
    {
        Close();
    }

    private static void Close() => SukiHost.CloseDialog();
}
