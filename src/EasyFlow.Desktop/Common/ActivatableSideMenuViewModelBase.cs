using Material.Icons;
using ReactiveUI;
using System.Reactive.Disposables;

namespace EasyFlow.Desktop.Common;

public class ActivatableSideMenuViewModelBase : SideMenuViewModelBase, IActivatableViewModel
{
    public ViewModelActivator Activator { get; } = new();

    public ActivatableSideMenuViewModelBase(string displayName, MaterialIconKind icon, int index = 0)
        : base(displayName, icon, index)
    {
        this.WhenActivated(disposables =>
        {
            this.HandleActivation(disposables);

            Disposable
                .Create(() => this.HandleDeactivation())
                .DisposeWith(disposables);
        });
    }

    public virtual void HandleActivation(CompositeDisposable d)
    {
    }

    public virtual void HandleDeactivation()
    {
    }
}