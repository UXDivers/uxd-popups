using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace UXDivers.Popups.Maui;

/// Windows-specific implementation of the native popup manager.
internal partial class NativePopupManager
{
    private PopupManager? _popupManager;
    private readonly object _popupManagerLock = new();

    /// <summary>
    /// Displays a native view for the given popup page.
    /// </summary>
    /// <param name="popupPage">The popup page to display.</param>
    /// <returns>A Task that resolves with the native view object.</returns>
    /// <exception cref="ArgumentException">Thrown if the popup cannot be converted to a native view.</exception>
    public async Task<object> ShowNativeViewAsync(PopupPage popupPage)
    {
        ArgumentNullException.ThrowIfNull(popupPage);

        var manager = GetOrCreatePopupManager();
        var nativeView = await manager.PushPopupAsync(popupPage);

        return nativeView;
    }

    /// <summary>
    /// Closes and removes the native view for the given popup.
    /// </summary>
    /// <param name="nativePopup">The native popup object to close.</param>
    /// <returns>A completed Task.</returns>
    /// <exception cref="ArgumentException">Thrown if the native popup is not a valid <see cref="FrameworkElement"/>.</exception>
    public Task CloseNativeViewAsync(object nativePopup)
    {
        if (nativePopup is not FrameworkElement nativeView)
        {
            throw new ArgumentException("Invalid native popup type, it should be or inherit from " + nameof(FrameworkElement));
        }

        PopupManager manager;

        lock (_popupManagerLock)
        {
            manager = _popupManager
                ?? throw new InvalidOperationException("PopupManager is not initialized. Cannot close popup that was never opened.");
        }

        return manager.PopPopupAsync(nativeView);
    }

    private Microsoft.Maui.Controls.Window GetWindow()
    {
        var application = Microsoft.Maui.Controls.Application.Current
            ?? throw new InvalidOperationException("Application.Current is null");

        var window = application.Windows.FirstOrDefault()
            ?? throw new InvalidOperationException("No active window found");

        return window;
    }

    private PopupManager GetOrCreatePopupManager()
    {
        if (_popupManager != null)
        {
            return _popupManager;
        }

        lock (_popupManagerLock)
        {
            if (_popupManager != null)
            {
                return _popupManager;
            }

            var window = GetWindow();
            _popupManager = new PopupManager(window);
            return _popupManager;
        }
    }

    private class PopupManager
    {
        private readonly Microsoft.Maui.Controls.Window _window;
        private readonly object _stateAccess = new();
        private readonly Dictionary<object, IMauiContext> _nativeViewsContexts = [];

        public PopupManager(Microsoft.Maui.Controls.Window window)
        {
            _window = window;
            _window.Destroying += OnWindowDestroying;

            Initialize();
        }

        /// <summary>
        /// Gets platform panel to be used for containing panels host.
        /// The resolved panel is expected to be <see cref="WindowRootViewContainer"/> used by <see cref="ModalNavigationManager"/>.
        /// </summary>
        protected Panel Container => GetContainer(MauiContext);

        protected IMauiContext? MauiContext => _window.Handler.MauiContext;

        protected PopupHost? Host { get; private set; }

        public Task<FrameworkElement> PushPopupAsync(PopupPage popupPage)
        {
            ArgumentNullException.ThrowIfNull(popupPage);

            if (MauiContext is null)
            {
                throw new InvalidOperationException("MauiContext not found");
            }

            if (Host is null)
            {
                throw new InvalidOperationException("Missing host setup");
            }

            lock (_stateAccess)
            {
                // Create a new scoped MauiContext that overrides NavigationRootManager.
                var scopedMauiContext = Utils.MakeScoped(MauiContext, registerNewNavigationRoot: true);
                var nativeView = GetNativeView(popupPage, scopedMauiContext);
                var navigationRootManager = GetNavigationRootManager(scopedMauiContext, nativeView);
                PreparePopup(scopedMauiContext, popupPage, navigationRootManager);

                Host.Push(navigationRootManager.RootView);

                _nativeViewsContexts.Add(nativeView, scopedMauiContext);

                return Task.FromResult(nativeView);
            }
        }

        public Task<bool> PopPopupAsync(FrameworkElement element)
        {
            if (Host is null)
            {
                throw new InvalidOperationException("Missing host setup");
            }

            lock (_stateAccess)
            {
                if (!_nativeViewsContexts.TryGetValue(element, out var scopedMauiContext))
                {
                    throw new InvalidOperationException("Could not find maui context for native popup");
                }

                var navigationRootManager = Utils.GetNavigationRootManager(scopedMauiContext);

                if (!Host.Pop(navigationRootManager.RootView))
                {
                    return Task.FromResult(false);
                }

                try
                {
                    navigationRootManager.Disconnect();

                    if (element is PopupBackgroundView nativeBackground)
                    {
                        nativeBackground.Dispose();
                    }
                }
                catch (Exception)
                {
                }
                finally
                {
                    _nativeViewsContexts.Remove(element);
                }

                return Task.FromResult(true);
            }
        }

        protected virtual void Initialize()
        {
            SetupHost();
        }

        protected virtual void Deinitialize()
        {
            lock (_stateAccess)
            {
                ClearHost();
                _nativeViewsContexts.Clear();
            }
        }

        private void SetupHost()
        {
            var container = Container;
            Host = new PopupHost();
            container.Children.Add(Host);
        }

        private void ClearHost()
        {
            if (Host is null)
            {
                return;
            }

            Container.Children.Remove(Host);
            Host = null;
        }

        private void OnWindowDestroying(object? sender, EventArgs e)
        {
            _window.Destroying -= OnWindowDestroying;
            Deinitialize();
        }

        private static FrameworkElement GetNativeView(PopupPage popupPage, IMauiContext scopedMauiContext)
        {
            var nativeView = popupPage.ToPlatform(scopedMauiContext);
            var nativeContent = popupPage.ActualContent?.ToPlatform(scopedMauiContext)
                ?? throw new ArgumentException("Popup content could not be converted to a native view");

            var nativeBackground = new PopupBackgroundView(
                nativeContent,
                async () => await popupPage.OnBackgroundTapped());

            nativeBackground.Children.Add(nativeView);

            return nativeBackground;
        }

        private static NavigationRootManager GetNavigationRootManager(IMauiContext scopedMauiContext, FrameworkElement nativeView)
        {
            // Connect the NavigationRootManager to the native view so that it's wrapped in a NavigationView that handles title bar configuration.
            var navigationRootManager = Utils.GetNavigationRootManager(scopedMauiContext);
            var navigationView = new PopupNavigationView() { Content = nativeView };
            navigationRootManager.Connect(navigationView);
            return navigationRootManager;
        }

        private static void PreparePopup(IMauiContext mauiContext, PopupPage popupPage, NavigationRootManager navigationRootManager)
        {
            var platformWindow = Utils.GetPlatformWindow(mauiContext);
            var titleBar = platformWindow.AppWindow.TitleBar;
            var windowBounds = platformWindow.Bounds;
            var bounds = new Rect(windowBounds.Left, windowBounds.Top, windowBounds.Width, windowBounds.Height - titleBar.Height);
            popupPage.Measure(bounds.Size.Width, bounds.Size.Height);
            popupPage.Arrange(bounds);
        }

        private static Panel GetContainer(IMauiContext? mauiContext)
        {
            ArgumentNullException.ThrowIfNull(mauiContext);

            var platformWindow = Utils.GetPlatformWindow(mauiContext);
            return platformWindow.Content as Panel ??
                throw new InvalidOperationException("Root container Panel not found");
        }

        /// <summary>
        /// Used to prevent the default gray overlay of NavigationView.
        /// </summary>
        protected class PopupNavigationView : NavigationView
        {
            public PopupNavigationView() 
            {
                DefaultStyleKey = typeof(PopupNavigationView);
            }
        }

        protected class PopupHost : Panel
        {
            public void Push(FrameworkElement frameworkElement) =>
                Children.Add(frameworkElement);

            public bool Pop(FrameworkElement frameworkElement) =>
                Children.Remove(frameworkElement);

            protected override Windows.Foundation.Size MeasureOverride(Windows.Foundation.Size availableSize)
            {
                var width = availableSize.Width;
                var height = availableSize.Height;

                if (double.IsInfinity(width))
                {
                    width = XamlRoot.Size.Width;
                }

                if (double.IsInfinity(height))
                {
                    height = XamlRoot.Size.Height;
                }

                var size = new Windows.Foundation.Size(width, height);

                foreach (var child in Children)
                {
                    child.Measure(size);
                }

                return size;
            }

            protected override Windows.Foundation.Size ArrangeOverride(Windows.Foundation.Size finalSize)
            {
                foreach (var child in Children)
                {
                    child.Arrange(new Windows.Foundation.Rect(new Windows.Foundation.Point(0, 0), finalSize));
                }

                return finalSize;
            }
        }
    }
}