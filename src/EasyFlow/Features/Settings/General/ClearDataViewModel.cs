using CommunityToolkit.Mvvm.Input;
using EasyFlow.Common;
using EasyFlow.Data;
using SukiUI.Controls;
using System;

namespace EasyFlow.Features.Settings.General;
public sealed partial class ClearDataViewModel : ViewModelBase
{
    private readonly IDatabaseManager _databaseMigrator;
    private readonly Action? _onOk;

    public ClearDataViewModel(IDatabaseManager databaseMigrator, Action? onOk = null)
    {
        _databaseMigrator = databaseMigrator;
        _onOk = onOk;
    }

    [RelayCommand]
    private void Ok()
    {
        try 
        {
            var result = _databaseMigrator.Reset();
            if (result && _onOk is not null)
            {
                _onOk();
            }
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
