using ReactiveUI;
using System.Reactive.Disposables;

namespace EasyFlow.Desktop.Common;

public class ActivatableViewModelBase : ViewModelBase, IActivatableViewModel
{
    public ViewModelActivator Activator { get; } = new();

    public ActivatableViewModelBase()
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