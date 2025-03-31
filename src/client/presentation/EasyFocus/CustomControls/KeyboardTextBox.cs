using Avalonia.Controls;
using Avalonia.Interactivity;
using EasyFocus.Domain.Services;
using Splat;
using System;
using System.Diagnostics;

namespace EasyFocus.CustomControls;

public class KeyboardTextBox : TextBox
{
    protected override Type StyleKeyOverride
    { get { return typeof(TextBox); } }

    public KeyboardTextBox()
    {
    }

    protected override void OnPointerPressed(Avalonia.Input.PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        var isTouch = e.Pointer.Type == Avalonia.Input.PointerType.Touch;
        if (!isTouch)
        {
            return;
        }

        if (OperatingSystem.IsAndroid() || OperatingSystem.IsIOS())
        {
            return;
        }

        var keyboard = Locator.Current.GetService<ICustomVirtualKeyboard>();

        if (keyboard is null)
        {
            Debug.WriteLine("Not supported");
            return;
        }

        keyboard.Open();
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {
        base.OnLostFocus(e);

        if (OperatingSystem.IsAndroid() || OperatingSystem.IsIOS())
        {
            return;
        }

        var keyboard = Locator.Current.GetService<ICustomVirtualKeyboard>();

        if (keyboard is null)
        {
            Debug.WriteLine("Not supported");
            return;
        }

        keyboard.Close();
    }
}