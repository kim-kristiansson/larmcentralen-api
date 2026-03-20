using System.Globalization;

namespace Larmcentralen.Maui.Converters;

public class SeverityToBackgroundConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.ToString() switch
        {
            "Låg" => Color.FromArgb("#DBEAFE"),
            "Medel" => Color.FromArgb("#FEF3C7"),
            "Hög" => Color.FromArgb("#FED7AA"),
            "Kritisk" => Color.FromArgb("#FECACA"),
            _ => Color.FromArgb("#F1F5F9")
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class SeverityToTextColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.ToString() switch
        {
            "Låg" => Color.FromArgb("#1E40AF"),
            "Medel" => Color.FromArgb("#92400E"),
            "Hög" => Color.FromArgb("#9A3412"),
            "Kritisk" => Color.FromArgb("#991B1B"),
            _ => Color.FromArgb("#64748B")
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class SeverityToIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.ToString() switch
        {
            "Låg" => "●",
            "Medel" => "▲",
            "Hög" => "◆",
            "Kritisk" => "⬣",
            _ => "●"
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}