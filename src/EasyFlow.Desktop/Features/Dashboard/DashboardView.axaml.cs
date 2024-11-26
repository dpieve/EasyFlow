using Avalonia.ReactiveUI;

namespace EasyFlow.Desktop.Features.Dashboard;

public partial class DashboardView : ReactiveUserControl<DashboardViewModel>
{
    public DashboardView()
    {
        InitializeComponent();
    }
}