using System;
using System.Globalization;
using System.Reflection;
using Avalonia.Data.Converters;
using Terminal.Core;
using Terminal.Core.Enums;

namespace Terminal.Converters;

/// <summary>
/// Конвертор элементов перечислений через автрибут FriendlyName.
/// </summary>
public class EnumFriendlyNameConverter : IValueConverter
{
    /// <summary>
    /// Конверитировать из элемента перечисления в название.
    /// </summary>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
            return string.Empty;

        var type = value.GetType();
        if (!type.IsEnum)
            return value.ToString() ?? string.Empty;

        var field = type.GetField(value.ToString()!);
        var attribute = field?.GetCustomAttribute<FriendlyNameAttribute>();
        
        return attribute?.Name ?? value.ToString() ?? string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}