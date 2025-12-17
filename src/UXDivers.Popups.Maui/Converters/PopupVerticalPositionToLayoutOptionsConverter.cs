using System.Globalization;

namespace UXDivers.Popups.Maui.Converters;

/// <summary>
/// Converts a <see cref="VerticalPosition"/> enum value to a <see cref="LayoutOptions"/> value.
/// </summary>
public class PopupVerticalPositionToLayoutOptionsConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is VerticalPosition position)
        {
            return position switch
            {
                VerticalPosition.Top => LayoutOptions.Start,
                VerticalPosition.Bottom => LayoutOptions.End,
                _ => LayoutOptions.Start
            };
        }

        return LayoutOptions.Start;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
