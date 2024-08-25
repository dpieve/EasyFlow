using CommunityToolkit.Mvvm.ComponentModel;
using Material.Icons;

namespace EasyFlow.Presentation.Common;

public abstract partial class PageViewModelBase(string displayName, MaterialIconKind icon, int index = 0) : ObservableRecipient
{
    [ObservableProperty] private string _displayName = displayName;
    [ObservableProperty] private MaterialIconKind _icon = icon;
    [ObservableProperty] private int _index = index;
}