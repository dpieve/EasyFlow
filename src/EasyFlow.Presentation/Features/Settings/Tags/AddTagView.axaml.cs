using Avalonia.Controls;
using Avalonia.Interactivity;

namespace EasyFlow.Presentation.Features.Settings.Tags;

public partial class AddTagView : UserControl
{
    public AddTagView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        var tb = TagNameTextBox;

        tb.Focus();

        tb.SelectionEnd = tb.Text is not null ? tb.Text.Length : 0;
        tb.SelectionStart = tb.SelectionEnd;
    }
}