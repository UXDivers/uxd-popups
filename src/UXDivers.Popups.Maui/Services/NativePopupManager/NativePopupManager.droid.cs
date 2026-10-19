using System.Runtime.CompilerServices;
using Android.Views;
using Microsoft.Maui.Platform;

namespace UXDivers.Popups.Maui;

/// Android-specific implementation of the native popup manager.
/// Handles the display and removal of native popups on Android.
internal partial class NativePopupManager
{
    private static readonly ConditionalWeakTable<Android.Views.View, SafeAreaObserver> _safeAreaObservers = new();

    /// <summary>
    /// Displays a native view for the given popup page.
    /// </summary>
    /// <param name="popup">The popup page to display.</param>
    /// <returns>A Task that resolves with the native view object.</returns>
    /// <exception cref="NullReferenceException">Thrown if the root view or native view is invalid.</exception>
    private Task<object> ShowPlatformNativeViewAsync(PopupPage popup)
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

        popup.Measure(widthDip, heightDip);
        popup.Arrange(new Rect(0, 0, widthDip, heightDip));

        var layoutParams = new ViewGroup.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.MatchParent);

        rootView.AddView(nativeView, layoutParams);

        // Keep safe-area padding and keyboard offset in sync with window insets
        var safeAreaObserver = new SafeAreaObserver(popup);
        safeAreaObserver.Start(nativeView);
        _safeAreaObservers.AddOrUpdate(nativeView, safeAreaObserver);

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

        // Stop observing window insets and release the observer
        if (_safeAreaObservers.TryGetValue(nativeView, out var safeAreaObserver))
        {
            safeAreaObserver.Stop();
            safeAreaObserver.Dispose();
            _safeAreaObservers.Remove(nativeView);
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
