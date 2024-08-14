using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyFlow.Common;
using ReactiveUI;
using SukiUI.Controls;

namespace EasyFlow.Features.Settings.Tags;

public sealed partial class TagItemViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _name;

    private readonly Tag _tag;

    public TagItemViewModel(Tag tag)
    {
        Name = tag.Name;
        _tag = tag;
    }

    [RelayCommand]
    private void EditTag()
    {
        SukiHost.ShowDialog(new AddTagViewModel(_tag, EditedTag), allowBackgroundClose: false);
    }

    private void EditedTag(Tag tag)
    {
        _tag.Name = tag.Name;
        Name = _tag.Name;

        SukiHost.ShowToast($"Updated tag {Name}", "Updated tag", SukiUI.Enums.NotificationType.Info);
    }

    [RelayCommand]
    private void DeleteTag()
    {
        SukiHost.ShowToast($"Delete tag {Name}", "Delete tag", SukiUI.Enums.NotificationType.Info);

        MessageBus.Current.SendMessage(new DeletedTagMessage(_tag));
    }
}