namespace UXDivers.Popups.Maui.Controls;

/// <summary>
/// A lightweight notification popup with an icon and title, typically shown briefly.
/// </summary>
public class Toast : PopupPage
{
    public static readonly BindableProperty IconTextProperty = BindableProperty.Create(
        nameof(IconText),
        typeof(string),
        typeof(Toast),
        null);

    /// <summary>
    /// Gets or sets the icon source displayed in the toast.
    /// </summary>
    public string IconText
    {
        get { return (string)GetValue(IconTextProperty); }
        set { SetValue(IconTextProperty, value); }
    }

    public static readonly BindableProperty IconColorProperty = BindableProperty.Create(
        nameof(IconColor),
        typeof(Color),
        typeof(Toast),
        null);

    /// <summary>
    /// Gets or sets the color of the toast icon.
    /// </summary>
    public Color IconColor
    {
        get { return (Color)GetValue(IconColorProperty); }
        set { SetValue(IconColorProperty, value); }
    }

    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title),
        typeof(string),
        typeof(Toast),
        null);

    /// <summary>
    /// Gets or sets the title text displayed in the toast.
    /// </summary>
    public string Title
    {
        get { return (string)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public static readonly BindableProperty VerticalPositionProperty = BindableProperty.Create(
        nameof(VerticalPosition),
        typeof(VerticalPosition),
        typeof(Toast),
        VerticalPosition.Top);

    /// <summary>
    /// Gets or sets the vertical position of the toast on the screen.
    /// Default value is <see cref="VerticalPosition.Top"/>.
    /// </summary>
    public VerticalPosition VerticalPosition
    {
        get { return (VerticalPosition)GetValue(VerticalPositionProperty); }
        set { SetValue(VerticalPositionProperty, value); }
    }
}