using AndroidX.Core.View;
using AView = Android.Views.View;

namespace UXDivers.Popups.Maui;

/// <summary>
/// Observes window insets on Android and keeps the popup's safe-area padding in sync (e.g. after rotation).
/// When AvoidKeyboard is enabled it also tracks the IME inset, since Android allows only one insets listener per view.
/// </summary>
internal class SafeAreaObserver : Java.Lang.Object, IOnApplyWindowInsetsListener
{
    private readonly PopupPage _popupPage;
    private AView? _view;

    public SafeAreaObserver(PopupPage popupPage)
    {
        _popupPage = popupPage;
    }

    public void Start(AView view)
    {
        _view = view;
        ViewCompat.SetOnApplyWindowInsetsListener(view, this);

        // Apply the current insets synchronously to avoid a frame with the wrong padding
        var currentInsets = ViewCompat.GetRootWindowInsets(view);
        if (currentInsets is not null)
        {
            ApplyInsets(view, currentInsets);
        }

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

    // On net10 and above, the method signature for IOnApplyWindowInsetsListener has changed to return a nullable WindowInsetsCompat
#if NET10_0_OR_GREATER
    public WindowInsetsCompat? OnApplyWindowInsets(AView? view, WindowInsetsCompat? insets)
    {
        if (view is null || insets is null)
        {
            _popupPage.UpdateKeyboardOffset(0);
            return insets;
        }
        
#else
    public WindowInsetsCompat OnApplyWindowInsets(AView view, WindowInsetsCompat insets)
    {
#endif
        ApplyInsets(view, insets);
        return insets;
    }

    private void ApplyInsets(AView view, WindowInsetsCompat insets)
    {
        var density = view.Context?.Resources?.DisplayMetrics?.Density ?? 1f;

        var systemBars = insets.GetInsets(WindowInsetsCompat.Type.SystemBars());
        var displayCutout = insets.GetInsets(WindowInsetsCompat.Type.DisplayCutout());

        _popupPage.ApplySafeAreaInsets(new Thickness(
            Math.Max(systemBars?.Left ?? 0, displayCutout?.Left ?? 0) / density,
            Math.Max(systemBars?.Top ?? 0, displayCutout?.Top ?? 0) / density,
            Math.Max(systemBars?.Right ?? 0, displayCutout?.Right ?? 0) / density,
            Math.Max(systemBars?.Bottom ?? 0, displayCutout?.Bottom ?? 0) / density));

        if (!_popupPage.AvoidKeyboard)
        {
            return;
        }

        double keyboardHeightPx = insets.GetInsets(WindowInsetsCompat.Type.Ime())?.Bottom ?? 0;

        // Subtract system bars bottom to avoid double-padding when safe area bottom is active
        if (_popupPage.SafeAreaAsPadding.HasFlag(SafeAreaAsPadding.Bottom))
        {
            keyboardHeightPx -= systemBars?.Bottom ?? 0;
        }

        _popupPage.UpdateKeyboardOffset(Math.Max(0, keyboardHeightPx) / density);
    }
}
