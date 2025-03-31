using Avalonia;
using Avalonia.Controls;

namespace EasyFocus.CustomControls;

public sealed partial class LabelsSlider : UserControl
{
    public LabelsSlider()
    {
        InitializeComponent();
    }

    public static readonly StyledProperty<string> LeftTextProperty =
        AvaloniaProperty.Register<LabelsSlider, string>(nameof(LeftText), string.Empty);

    public string LeftText
    {
        get => this.GetValue(LeftTextProperty);
        set => SetValue(LeftTextProperty, value);
    }

    public static readonly StyledProperty<string> RightTextProperty =
        AvaloniaProperty.Register<LabelsSlider, string>(nameof(RightText), string.Empty);

    public string RightText
    {
        get => this.GetValue(RightTextProperty);
        set => SetValue(RightTextProperty, value);
    }

    public static readonly StyledProperty<double> SliderValueProperty =
        AvaloniaProperty.Register<LabelsSlider, double>(nameof(SliderValue), 0.0);

    public double SliderValue
    {
        get => this.GetValue(SliderValueProperty);
        set => SetValue(SliderValueProperty, value);
    }

    public static readonly StyledProperty<double> MinimumProperty =
        AvaloniaProperty.Register<LabelsSlider, double>(nameof(Minimum), 0.0);

    public double Minimum
    {
        get => this.GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public static readonly StyledProperty<double> MaximumProperty =
        AvaloniaProperty.Register<LabelsSlider, double>(nameof(Maximum), 100.0);

    public double Maximum
    {
        get => this.GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }
}