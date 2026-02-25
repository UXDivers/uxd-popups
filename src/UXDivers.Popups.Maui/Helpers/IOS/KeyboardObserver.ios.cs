using Foundation;
using UIKit;

namespace UXDivers.Popups.Maui;

/// <summary>
/// Observes keyboard visibility changes on iOS and updates the popup content's position accordingly.
/// Animates the position change synchronized with the keyboard animation using TranslationY.
/// Subtracts safe area bottom insets when SafeAreaAsPadding.Bottom is active to avoid double-offsetting.
/// </summary>
internal class KeyboardObserver : IDisposable
{
    private readonly PopupPage _popupPage;
    private NSObject? _willShowObserver;
    private NSObject? _willHideObserver;

    public KeyboardObserver(PopupPage popupPage)
    {
        _popupPage = popupPage;
    }

    public void Start()
    {
        _willShowObserver = UIKeyboard.Notifications.ObserveWillShow(OnKeyboardWillShow);
        _willHideObserver = UIKeyboard.Notifications.ObserveWillHide(OnKeyboardWillHide);
    }

    public void Stop()
    {
        _willShowObserver?.Dispose();
        _willShowObserver = null;

        _willHideObserver?.Dispose();
        _willHideObserver = null;

        _popupPage.UpdateKeyboardOffset(0);
    }

    private void OnKeyboardWillShow(object? sender, UIKeyboardEventArgs e)
    {
        double keyboardHeight = e.FrameEnd.Height;

        // Subtract safe area bottom to avoid double-padding when safe area bottom is active
        if (_popupPage.SafeAreaAsPadding.HasFlag(SafeAreaAsPadding.Bottom))
        {
            var window = Utils.GetMainWindow();
            if (window != null)
            {
                keyboardHeight -= window.SafeAreaInsets.Bottom;
            }
        }

        keyboardHeight = Math.Max(0, keyboardHeight);

        UIView.Animate(
            e.AnimationDuration,
            0,
            (UIViewAnimationOptions)((int)e.AnimationCurve << 16),
            () => _popupPage.UpdateKeyboardOffset(keyboardHeight),
            () => { }
        );
    }

    private void OnKeyboardWillHide(object? sender, UIKeyboardEventArgs e)
    {
        UIView.Animate(
            e.AnimationDuration,
            0,
            (UIViewAnimationOptions)((int)e.AnimationCurve << 16),
            () => _popupPage.UpdateKeyboardOffset(0),
            () => { }
        );
    }

    public void Dispose()
    {
        Stop();
    }
}
