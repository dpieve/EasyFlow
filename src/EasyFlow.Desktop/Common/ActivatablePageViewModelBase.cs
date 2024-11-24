using ReactiveUI;
using System.Reactive.Disposables;

namespace EasyFlow.Desktop.Common;
public class ActivatablePageViewModelBase : PageViewModelBase, IActivatableViewModel
{
    public ViewModelActivator Activator { get; } = new();

    public ActivatablePageViewModelBase(IScreen hostScreen, string urlPathSegment)
        : base(hostScreen, urlPathSegment)
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