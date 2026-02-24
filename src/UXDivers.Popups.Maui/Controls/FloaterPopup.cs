namespace UXDivers.Popups.Maui.Controls;

/// <summary>
/// A floating popup with an icon, title, and text content, typically displayed at a specific position.
/// </summary>
public class FloaterPopup : PopupPage
{
    public static readonly BindableProperty IconTextProperty = BindableProperty.Create(
        nameof(IconText),
        typeof(string),
        typeof(FloaterPopup),
        null);

    /// <summary>
    /// Gets or sets the icon source displayed in the popup.
    /// </summary>
    public string IconText
    {
        get { return (string)GetValue(IconTextProperty); }
        set { SetValue(IconTextProperty, value); }
    }

    public static readonly BindableProperty IconColorProperty = BindableProperty.Create(
        nameof(IconColor),
        typeof(Color),
        typeof(FloaterPopup),
        null);

    /// <summary>
    /// Gets or sets the color of the icon.
    /// </summary>
    public Color IconColor
    {
        get { return (Color)GetValue(IconColorProperty); }
        set { SetValue(IconColorProperty, value); }
    }

    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title),
        typeof(string),
        typeof(FloaterPopup),
        null);

    /// <summary>
    /// Gets or sets the title text displayed in the popup.
    /// </summary>
    public string Title
    {
        get { return (string)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(FloaterPopup),
        null);

    /// <summary>
    /// Gets or sets the main text content displayed in the popup.
    /// </summary>
    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public static readonly BindableProperty VerticalPositionProperty = BindableProperty.Create(
        nameof(VerticalPosition),
        typeof(VerticalPosition),
        typeof(FloaterPopup),
        VerticalPosition.Top);

    /// <summary>
    /// Gets or sets the vertical position of the popup on the screen.
    /// Default value is <see cref="VerticalPosition.Top"/>.
    /// </summary>
    public VerticalPosition VerticalPosition
    {
        get { return (VerticalPosition)GetValue(VerticalPositionProperty); }
        set { SetValue(VerticalPositionProperty, value); }
    }
}