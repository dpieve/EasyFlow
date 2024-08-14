using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyFlow.Common;
using SukiUI.Controls;
using System;

namespace EasyFlow.Features.Settings.Tags;

public sealed partial class AddTagViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _tagName;

    private readonly Tag _tag;
    private readonly Action<Tag>? _returnedTag;

    public AddTagViewModel(Tag? tag = null, Action<Tag>? returnedTag = null)
    {
        _tag = tag ?? new(string.Empty);
        _returnedTag = returnedTag;
        TagName = _tag.Name;
    }

    [RelayCommand]
    private static void CloseDialog() => SukiHost.CloseDialog();

    [RelayCommand]
    private void OkButton()
    {
        _tag.Name = TagName;

        if (_returnedTag is not null)
        {
            _returnedTag(_tag);
        }

        SukiHost.ShowToast("Saving tag", $"Tag {_tag.Name}", SukiUI.Enums.NotificationType.Success);

        CloseDialog();
    }
}