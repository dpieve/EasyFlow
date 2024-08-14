using CommunityToolkit.Mvvm.Input;
using EasyFlow.Common;
using ReactiveUI;
using SukiUI.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace EasyFlow.Features.Settings.Tags;
public partial class TagsViewModel : ViewModelBase
{
    public TagsViewModel()
    {
        MessageBus.Current.Listen<DeletedTagMessage>().Subscribe(Receive);
    }

    public ObservableCollection<TagItemViewModel> Tags { get; } = [];

    public void Activate()
    {
    }
    
    public void Deactivate()
    {
    }

    [RelayCommand]
    private async Task AddTag()
    {
        SukiHost.ShowDialog(new AddTagViewModel(returnedTag: AddedTag), allowBackgroundClose: false);
        await Realod();
    }

    private void AddedTag(Tags.Tag tag)
    {
        Tags.Add(new TagItemViewModel(tag));
    }

    private Task Realod()
    {
        return Task.CompletedTask;
    }

    private List<TagItemViewModel> CreateTags()
    {
        return new List<TagItemViewModel>
        {
            new TagItemViewModel(new Tags.Tag("Study")),
            new TagItemViewModel(new Tags.Tag("Work")),
            new TagItemViewModel(new Tags.Tag("Read")),
            new TagItemViewModel(new Tags.Tag("Meditate")),
        };
    }

    public void Receive(DeletedTagMessage message)
    {
        var tag = Tags.FirstOrDefault(t => t.Name == message.Value.Name);
        if (tag is not null)
        {
            Tags.Remove(tag);
        }
    }
}
