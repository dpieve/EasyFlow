using ReactiveUI;

namespace EasyFlow.Desktop.Common;
internal class ActivatableViewModel : IActivatableViewModel
{
    public ViewModelActivator Activator { get; } = new();
}
