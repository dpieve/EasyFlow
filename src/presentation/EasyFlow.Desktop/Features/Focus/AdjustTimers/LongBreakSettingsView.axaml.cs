using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using System.Linq;

namespace EasyFlow.Desktop.Features.Focus.AdjustTimers;

public partial class LongBreakSettingsView : UserControl
{
    public LongBreakSettingsView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        var tb = LongBreakSessionsNumericUpDown
                    .GetTemplateChildren()
                    .OfType<TextBox>()
                    .First();

        tb.Focus();

        tb.SelectionEnd = tb.Text is not null ? tb.Text.Length : 0;
        tb.SelectionStart = tb.SelectionEnd;
    }
}