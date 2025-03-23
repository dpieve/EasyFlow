using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;

namespace EasyFlow.Features;

public partial class MainWindow : ReactiveWindow<MainViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            return;
        }

        if (mainWindow.WindowState == WindowState.FullScreen)
        {
            return;
        }

        bool isControl = IsControl(e.Source as Visual);
        if (isControl)
        {
            return;
        }

        BeginMoveDrag(e);
    }

    private void Window_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        bool isControl = IsControl(e.Source as Visual);
        if (isControl)
        {
            return;
        }

        if (e.Key == Key.F && mainWindow.WindowState != WindowState.FullScreen)
        {
            mainWindow.WindowState = WindowState.FullScreen;
        }

        if (e.Key == Key.Escape && mainWindow.WindowState != WindowState.Normal)
        {
            mainWindow.WindowState = WindowState.Normal;
        }
    }

    private static bool IsControl(Visual? visual)
    {
        if (visual is null)
        {
            return false;
        }

        while (visual is not null)
        {
            if (visual is ComboBox ||
                visual is TextBox ||
                visual is Button ||
                visual is Slider ||
                visual is Border)
            {
                return true;
            }
            visual = visual.GetVisualParent();
        }

        return false;
    }

    private void Window_SizeChanged(object? sender, Avalonia.Controls.SizeChangedEventArgs e)
    {
        bool isSmallWindow = mainWindow.Width < 500 && mainWindow.Height < 500;
        SetSmallWindowMode(isSmallWindow);
    }

    private void SetSmallWindowMode(bool isSmallMode)
    {
        TitleBarButtons.IsVisible = !isSmallMode;
        mainWindow.Topmost = isSmallMode;

        if (ViewModel is not null)
        {
            ViewModel.Pomodoro.SmallWindowMode = isSmallMode;
        }
    }

    private void Button_Close_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        mainWindow.Close();
    }

    private void Button_Minimize_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        mainWindow.WindowState = WindowState.Minimized;
    }

    private void Button_Maximize_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (mainWindow.WindowState == WindowState.Normal)
        {
            mainWindow.WindowState = WindowState.Maximized;
        }
        else if (mainWindow.WindowState == WindowState.Maximized)
        {
            mainWindow.WindowState = WindowState.FullScreen;
        }
        else
        {
            mainWindow.WindowState = WindowState.Normal;
        }
    }

    private void Window_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        bool isControl = IsControl(e.Source as Visual);
        if (isControl)
        {
            return;
        }

        if (mainWindow.WindowState == WindowState.Maximized || mainWindow.WindowState == WindowState.FullScreen)
        {
            mainWindow.WindowState = WindowState.Normal;
        }
        else if (mainWindow.WindowState == WindowState.Normal)
        {
            mainWindow.WindowState = WindowState.FullScreen;
        }
    }
}