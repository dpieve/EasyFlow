using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Globalization;

namespace EasyFocus.Converters;

public sealed class StringToImageBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string path)
        {
            return null;
        }

        if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        try
        {
            var uri = AssetLoader.Open(new Uri($"avares://EasyFocus/Assets/{path}"));
            return new Bitmap(uri);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load image: {ex.Message}");
        }
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}