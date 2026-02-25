using AndroidX.Core.View;
using AView = Android.Views.View;

namespace UXDivers.Popups.Maui;

/// <summary>
/// Observes keyboard (IME) visibility changes on Android and updates the popup content's position accordingly.
/// Uses WindowInsetsCompat to detect IME insets and subtracts system bar insets when SafeAreaAsPadding.Bottom
/// is active to avoid double-offsetting.
/// </summary>
internal class KeyboardObserver : Java.Lang.Object, IOnApplyWindowInsetsListener
{
    private readonly PopupPage _popupPage;
    private AView? _view;

    public KeyboardObserver(PopupPage popupPage)
    {
        _popupPage = popupPage;
    }

    public void Start(AView view)
    {
        _view = view;
        ViewCompat.SetOnApplyWindowInsetsListener(view, this);
        ViewCompat.RequestApplyInsets(view);
    }

    public void Stop()
    {
        if (_view != null)
        {
            ViewCompat.SetOnApplyWindowInsetsListener(_view, null);
            _view = null;
        }

        _popupPage.UpdateKeyboardOffset(0);
    }

    public WindowInsetsCompat OnApplyWindowInsets(AView view, WindowInsetsCompat insets)
    {
        var imeInsets = insets.GetInsets(WindowInsetsCompat.Type.Ime());

        double keyboardHeightPx = imeInsets.Bottom;

        // Subtract system bars bottom to avoid double-padding when safe area bottom is active
        if (_popupPage.SafeAreaAsPadding.HasFlag(SafeAreaAsPadding.Bottom))
        {
            var systemBarsInsets = insets.GetInsets(WindowInsetsCompat.Type.SystemBars());
            keyboardHeightPx -= systemBarsInsets.Bottom;
        }

        keyboardHeightPx = Math.Max(0, keyboardHeightPx);

        // Convert pixels to DIPs
        var density = view.Context?.Resources?.DisplayMetrics?.Density ?? 1f;
        var keyboardHeightDip = keyboardHeightPx / density;

        _popupPage.UpdateKeyboardOffset(keyboardHeightDip);

        return insets;
    }
}
