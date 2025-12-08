using System.Globalization;
using Microsoft.Maui.Controls.Shapes;

namespace UXDivers.Popups.Maui.Converters;

public class CornerRadiusToStrokeShapeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if(value is not CornerRadius cornerRadius)
        {
            return null;
        }

        return new RoundRectangle()
        {
            CornerRadius = cornerRadius
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if(value is not RoundRectangle roundRectangle)
        {
            return null;
        }

        return roundRectangle.CornerRadius;
    }
}