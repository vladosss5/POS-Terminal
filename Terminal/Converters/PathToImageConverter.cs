using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Terminal.Converters;

/// <summary>
/// Конвертор названия изображения в изображение из Assets.
/// </summary>
public class PathToImageConverter : IValueConverter
{
    /// <summary>
    /// Конвертировать.
    /// </summary>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string path || string.IsNullOrEmpty(path)) return null;
        try
        {
            var uri = new Uri($"avares://Terminal/Assets/{path}");
            using var stream = AssetLoader.Open(uri);
            return new Bitmap(stream);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Конвертировать наоборот. Не реализовано.
    /// </summary>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}