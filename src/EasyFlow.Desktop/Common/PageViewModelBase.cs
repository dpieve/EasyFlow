using ReactiveUI;

namespace EasyFlow.Desktop.Common;
public class PageViewModelBase(IScreen hostScreen, string urlPathSegment)
        : ViewModelBase, IRoutableViewModel
{
    public IScreen HostScreen { get; } = hostScreen;
    public string? UrlPathSegment { get; } = urlPathSegment;
}