using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.VisualTree;
using System.Diagnostics;
using System.Linq;

namespace EasyFlow.Features.Report;

public partial class ReportView : UserControl
{
    public ReportView()
    {
        InitializeComponent();
    }

    private void TextBox_LostFocus(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var vm = DataContext as ReportViewModel;
        if (vm is null)
        {
            return;
        }

        var selectedItems = vm.Sessions.Where(i => i.IsEditing);
        foreach (var selectedItem in selectedItems)
        {
            selectedItem.IsEditing = false;
        }
    }

    private void DataGrid_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is not DataGrid dataGrid || dataGrid.SelectedItem is not SessionItemViewModel selectedSession)
        {
            return;
        }

        var rows = dataGrid.GetVisualDescendants().OfType<DataGridRow>();
        foreach (var row in rows)
        {
            var vm = row.DataContext as SessionItemViewModel;
            if (vm is not null && vm.IsEditing)
            {
                vm.IsEditing = false;
            }
        }
    }

    private void ScrollViewer_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        var selectedItem = SessionsDataGrid.SelectedItem;
        if (selectedItem is null)
        {
            return;
        }

        var selectedSession = selectedItem as SessionItemViewModel;
        if (selectedSession is null)
        {
            return;
        }

        selectedSession.IsEditing = true;
    }
 }