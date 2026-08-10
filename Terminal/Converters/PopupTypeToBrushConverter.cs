using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Terminal.Core.Enums;

namespace Terminal.Converters;

/// <summary>
/// Конвертер типа уведомления в цвет фона.
/// </summary>
public class PopupTypeToBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is PopupType type)
        {
            return type switch
            {
                PopupType.Error => new SolidColorBrush(Colors.Red),
                PopupType.Info => new SolidColorBrush(Colors.Orange),
                PopupType.Success => new SolidColorBrush(Colors.Green),
                _ => new SolidColorBrush(Colors.Gray)
            };
        }
        return new SolidColorBrush(Colors.Gray);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) 
        => throw new NotImplementedException();
}