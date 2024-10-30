using Material.Icons;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace EasyFlow.Desktop.Common;

public abstract partial class PageViewModelBase(string displayName, MaterialIconKind icon, int index = 0) : ReactiveObject
{
    [Reactive] private string _displayName = displayName;
    [Reactive] private MaterialIconKind _icon = icon;
    [Reactive] private int _index = index;
}