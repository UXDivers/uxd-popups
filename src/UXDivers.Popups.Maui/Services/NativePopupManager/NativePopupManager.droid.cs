using System.Runtime.CompilerServices;
using Android.Views;
using Microsoft.Maui.Platform;

namespace UXDivers.Popups.Maui;

/// Android-specific implementation of the native popup manager.
/// Handles the display and removal of native popups on Android.
internal partial class NativePopupManager
{
    private static readonly ConditionalWeakTable<Android.Views.View, KeyboardObserver> _keyboardObservers = new();

    /// <summary>
    /// Displays a native view for the given popup page.
    /// </summary>
    /// <param name="popup">The popup page to display.</param>
    /// <returns>A Task that resolves with the native view object.</returns>
    /// <exception cref="NullReferenceException">Thrown if the root view or native view is invalid.</exception>
    public Task<object> ShowNativeViewAsync(PopupPage popup)
    {
        if (popup == null)
        {
            throw new ArgumentNullException(nameof(popup));
        }

        var mauiContext = GetMauiContext();

        // Convert the popup page to a native view
        var nativeView = popup.ToPlatform(mauiContext);

        // Get the root view of the current activity
        var rootView = GetRootView();
        if (rootView == null)
        {
            throw new NullReferenceException("Couldn't find a root view in the visual tree");
        }

        // Ensure the native view is a ViewGroup
        if (nativeView is not ViewGroup group)
        {
            throw new NullReferenceException("Native view must be a ViewGroup with at least one child");
        }

        // Get the content of the popup
        var child = popup.ActualContent?.ToPlatform(mauiContext);

        if (child == null)
        {
            throw new NullReferenceException("Popup content could not be converted to a native view");
        }

        // Set a touch listener for background interactions
        nativeView.SetOnTouchListener(new BackgroundTouchListener(popup, group, child));

        // Measure and arrange the MAUI view before adding the native view to the root view
        float density = rootView.Context?.Resources?.DisplayMetrics?.Density ?? 1;
        double widthDip = rootView.Width / density;
        double heightDip = rootView.Height / density;

        // Get window insets for safe area handling
        WindowInsets? insets = null;
        if (OperatingSystem.IsAndroidVersionAtLeast(23))
        {
            insets = rootView.RootWindowInsets;
        }

        // Calculate safe area adjustments
        double topInset = 0, bottomInset = 0, leftInset = 0, rightInset = 0;

        if (insets != null)
        {
            if (OperatingSystem.IsAndroidVersionAtLeast(30))
            {
                // Modern API (Android 11+) - Get system bars and display cutout insets
                var systemBars = insets.GetInsets(WindowInsets.Type.SystemBars());
                var displayCutout = insets.GetInsets(WindowInsets.Type.DisplayCutout());

                topInset = Math.Max(systemBars.Top, displayCutout.Top) / density;
                bottomInset = Math.Max(systemBars.Bottom, displayCutout.Bottom) / density;
                leftInset = Math.Max(systemBars.Left, displayCutout.Left) / density;
                rightInset = Math.Max(systemBars.Right, displayCutout.Right) / density;
            }
            else
            {
                // Legacy API (Android 6.0 - 10) - Use deprecated system window insets
#pragma warning disable CS0618 // Type or member is obsolete
                topInset = insets.SystemWindowInsetTop / density;
                bottomInset = insets.SystemWindowInsetBottom / density;
                leftInset = insets.SystemWindowInsetLeft / density;
                rightInset = insets.SystemWindowInsetRight / density;
#pragma warning restore CS0618 // Type or member is obsolete
            }
        }

        popup.Measure(widthDip, heightDip);
        popup.Arrange(new Rect(0, 0, widthDip, heightDip));

        var layoutParams = new ViewGroup.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.MatchParent);

        // Apply safe area padding to the native view based on SafeAreaInsets property
        var popupSafeAreaInsets = popup.SafeAreaAsPadding;
        var finalTopInset = popupSafeAreaInsets.HasFlag(SafeAreaAsPadding.Top) ? topInset : 0;
        var finalLeftInset = popupSafeAreaInsets.HasFlag(SafeAreaAsPadding.Left) ? leftInset : 0;
        var finalRightInset = popupSafeAreaInsets.HasFlag(SafeAreaAsPadding.Right) ? rightInset : 0;
        var finalBottomInset = popupSafeAreaInsets.HasFlag(SafeAreaAsPadding.Bottom) ? bottomInset : 0;

        // nativeView.SetPadding(finalLeftInset, finalTopInset, finalRightInset, finalBottomInset);
        popup.Padding = new Thickness(
            popup.Padding.Left + finalLeftInset,
            popup.Padding.Top + finalTopInset,
            popup.Padding.Right + finalRightInset,
            popup.Padding.Bottom + finalBottomInset
        );

        rootView.AddView(nativeView, layoutParams);

        if (popup.AvoidKeyboard)
        {
            var keyboardObserver = new KeyboardObserver(popup);
            keyboardObserver.Start(nativeView);
            _keyboardObservers.AddOrUpdate(nativeView, keyboardObserver);
        }

        return Task.FromResult<object>(nativeView);
    }

    /// <summary>
    /// Closes and removes the native view for the given popup.
    /// </summary>
    /// <param name="nativePopup">The native popup object to close.</param>
    /// <returns>A completed Task.</returns>
    /// <exception cref="ArgumentException">Thrown if the native popup is not a valid Android view.</exception>
    public Task CloseNativeViewAsync(object nativePopup)
    {
        var nativeView = nativePopup as Android.Views.View;

        if (nativeView == null)
        {
            throw new ArgumentException("Invalid native popup type, it should be or inherit from Android.Views.View");
        }

        if (nativeView.Handle == IntPtr.Zero)
        {
            // The view has already been disposed
            return Task.CompletedTask;
        }

        // Stop keyboard observer if active
        if (_keyboardObservers.TryGetValue(nativeView, out var keyboardObserver))
        {
            keyboardObserver.Stop();
            keyboardObserver.Dispose();
            _keyboardObservers.Remove(nativeView);
        }

        // Remove the native view from its parent and dispose of it
        nativeView.RemoveFromParent();
        nativeView.Dispose();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Retrieves the root view of the current activity.
    /// </summary>
    /// <returns>The root view of the activity.</returns>
    /// <exception cref="NullReferenceException">Thrown if the activity or root view is null.</exception>
    private ViewGroup GetRootView()
    {
        // 1. Find the topmost displayed page (modal if present)
        Page? displayedRootPage = null;

        if (Application.Current?.Windows.Count > 0)
        {
            var window = Application.Current.Windows[0];
            // Check for modal stack first
            if (window.Navigation.ModalStack.Count > 0)
            {
                displayedRootPage = GetTopMostPage(window.Navigation.ModalStack.Last());
            }
            else
            {
                displayedRootPage = GetTopMostPage(window.Page);
            }
        }

        var frameLayout = FindTopFrameLayout(displayedRootPage?.Handler?.PlatformView as Android.Views.View);;

        if (frameLayout != null)
        {
            return frameLayout;
        }
        
        // Fallback to the main activity's decor view
        var activity = Platform.CurrentActivity;
        var decorView = activity?.Window?.DecorView as ViewGroup;
        if (decorView == null)
        {
            throw new NullReferenceException("Could not find a suitable root view.");
        }
        return decorView;
    }

    private Android.Widget.FrameLayout? FindTopFrameLayout(Android.Views.View? platformView)
    {
        if (platformView == null)
        {
            return null;
        }

        var parent = platformView.Parent;

        //Finds the coordinator layout because we want the FrameLayout that is wrapping it.
        while (parent != null && parent is not AndroidX.CoordinatorLayout.Widget.CoordinatorLayout)
        {
            parent = parent.Parent;
        }

        //Finds the FrameLayout parent of the coordinator layout
        while (parent != null && parent is not Android.Widget.FrameLayout)
        {
            parent = parent.Parent;
        }

        return parent as Android.Widget.FrameLayout;
    }
    
    private Page? GetTopMostPage(Page? page)
    {
        if (page is FlyoutPage flyoutPage)
        {
            return GetTopMostPage(flyoutPage.Detail);
        }

        if (page is TabbedPage tabbedPage)
        {
            return GetTopMostPage(tabbedPage.CurrentPage);
        }

        if (page is NavigationPage navigationPage)
        {
            return GetTopMostPage(navigationPage.CurrentPage);
        }

        return page;
    }
}
