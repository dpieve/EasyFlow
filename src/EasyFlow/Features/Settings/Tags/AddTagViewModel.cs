using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyFlow.Common;
using EasyFlow.Data;
using EasyFlow.Services;
using SukiUI.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EasyFlow.Features.Settings.Tags;

public sealed partial class AddTagViewModel : ViewModelBase
{
    private readonly ITagService _tagService;
    private readonly Tag _tag;
    private readonly Action<Tag> _onOk;

    [ObservableProperty]
    private string _tagName;
    
    public AddTagViewModel(
        ITagService tagService,
        Action<Tag> onOk,
        Tag? initialTag = null)
    {
        _tagService = tagService;
        _onOk = onOk;
        _tag = initialTag ?? new() { }; 

        TagName = _tag.Name;
    }

    [RelayCommand]
    private async Task OkButton()
    {
        if (string.IsNullOrEmpty(TagName) 
            || string.IsNullOrWhiteSpace(TagName)
            || TagName.Length < 3
            || TagName.Length > 90
            || TagName.Any(ch => !char.IsLetterOrDigit(ch)))
        {
            await SukiHost.ShowToast("Invalid tag name", "Try a different name.", SukiUI.Enums.NotificationType.Warning);
            CloseDialog();
            return;
        }

        _tag.Name = TagName;

        var saved = await PersistTag();
        if (!saved)
        {
            await SukiHost.ShowToast("Failed to save tag", "Failed to add the new tag to the database.", SukiUI.Enums.NotificationType.Error);
            CloseDialog();
            return;
        }

        _onOk(_tag);
        CloseDialog();
    }

    [RelayCommand]
    private static void CloseDialog() => SukiHost.CloseDialog();

    private async Task<bool> PersistTag()
    {
        var result = _tag.IsNew() ? 
            await _tagService.CreateAsync(_tag) : 
            await _tagService.UpdateAsync(_tag);
        
        if (result.Error is not null)
        {
            return false;
        }

        return true;
    }

}