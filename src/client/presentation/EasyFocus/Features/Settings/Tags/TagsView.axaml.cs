using Avalonia.ReactiveUI;

namespace EasyFocus.Features.Settings.Tags;

public partial class TagsView : ReactiveUserControl<TagsViewModel>
{
    public TagsView()
    {
        InitializeComponent();
    }

    private void TextBox_LostFocus(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        StopEditingTags();
    }

    private void ListBox_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
    {
        StopEditingTags();
    }

    private void StopEditingTags()
    {
        if (ViewModel is null)
        {
            return;
        }

        foreach (var tag in ViewModel.Tags)
        {
            if (tag.IsEditing)
            {
                tag.IsEditing = false;
            }
        }
    }
}