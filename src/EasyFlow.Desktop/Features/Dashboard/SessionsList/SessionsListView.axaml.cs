using Avalonia.Controls;
using System.Linq;

namespace EasyFlow.Desktop.Features.Dashboard.SessionsList;

public partial class SessionsListView : UserControl
{
    public SessionsListView()
    {
        InitializeComponent();
    }

    private void TextBox_LostFocus(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var vm = DataContext as SessionsListViewModel;
        if (vm is null)
        {
            return;
        }

        var selectedItem = vm.Items.FirstOrDefault(i => i.IsEditing);
        if (selectedItem is null)
        {
            return;
        }

        selectedItem.IsEditing = false;
    }
}