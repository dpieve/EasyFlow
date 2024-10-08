using Avalonia.Controls;
using Avalonia.Interactivity;

namespace EasyFlow.Desktop.Features.Focus.RunningTimer;

public partial class EditDescriptionView : UserControl
{
    public EditDescriptionView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        var tb = DescriptionTextBox;

        tb.Focus();

        tb.SelectionEnd = tb.Text is not null ? tb.Text.Length : 0;
        tb.SelectionStart = tb.SelectionEnd;
    }
}