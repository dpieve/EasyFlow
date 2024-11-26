using Avalonia.Controls;
using Avalonia.ReactiveUI;

namespace EasyFlow.Desktop.Features.Focus.RunningTimer;

public partial class RunningTimerView : ReactiveUserControl<RunningTimerViewModel>
{
    public RunningTimerView()
    {
        InitializeComponent();
    }
}