namespace UXDivers.Popups.Services;

/// <summary>
/// Interface defining the contract for the popup service.
/// </summary>
public interface IPopupService
{
    public static IPopupService Current { get; set; } = null!;

    /// <summary>
    /// Pushes a popup page onto the stack and displays it.
    /// </summary>
    /// <param name="popupPage">The popup page to display.</param>
    /// <param name="parameters">The navigation parameters to pass to the popup (optional).</param>
    /// <param name="waitUntilClosed">If false, the task completes immediately after the popup is shown; otherwise, it completes when the popup is closed.</param>
    /// <returns>A Task that completes when the popup is closed.</returns>
    Task PushAsync(IPopupPage popupPage, Dictionary<string, object?>? parameters = null, bool waitUntilClosed = true);

    /// <summary>
    /// Pushes a popup page with a result onto the stack and displays it.
    /// </summary>
    /// <typeparam name="T">The type of the result returned by the popup.</typeparam>
    /// <param name="popupPage">The popup page to display.</param>
    /// <param name="parameters">The navigation parameters to pass to the popup (optional).</param>
    /// <returns>A Task that resolves with the result of the popup.</returns>
    Task<T?> PushAsync<T>(IPopupResultPage<T> popupPage, Dictionary<string, object?>? parameters = null);

    /// <summary>
    /// Pushes a popup of the specified type onto the stack and displays it.
    /// The popup is resolved from the dependency injection container.
    /// </summary>
    /// <typeparam name="TPopup">The type of popup to display.</typeparam>
    /// <param name="parameters">The navigation parameters to pass to the popup (optional).</param>
    /// <param name="waitUntilClosed">If false, the task completes immediately after the popup is shown; otherwise, it completes when the popup is closed.</param>
    /// <returns>A Task that completes when the popup is closed.</returns>
    Task PushAsync<TPopup>(Dictionary<string, object?>? parameters = null, bool waitUntilClosed = true) where TPopup : class, IPopupPage;

    /// <summary>
    /// Pushes a popup with result of the specified type onto the stack and displays it.
    /// The popup is resolved from the dependency injection container.
    /// </summary>
    /// <typeparam name="TPopupResult">The type of popup with result to display.</typeparam>
    /// <typeparam name="TResult">The type of the result returned by the popup.</typeparam>
    /// <param name="parameters">The navigation parameters to pass to the popup (optional).</param>
    /// <returns>A Task that resolves with the result of the popup.</returns>
    Task<TResult?> PushAsync<TPopupResult, TResult>(Dictionary<string, object?>? parameters = null) 
        where TPopupResult : class, IPopupResultPage<TResult>;

    /// <summary>
    /// Pops the specified popup page from the stack.
    /// </summary>
    /// <param name="popupPage">
    /// The popup page to remove from the stack. If null, the top popup is popped.
    /// </param>
    /// <returns>A Task that completes when the popup is removed.</returns>
    Task PopAsync(IPopupPage? popupPage = null);

    /// <summary>
    /// Initializes the popup service with the required platform and UI dependencies. 
    /// This method must be called before using any popup functionality.
    /// </summary>
    void Initialize(INativePopupManager nativePopupManager, IUIThreadDispatcher uiThreadDispatcher, IViewModelAssignmentStrategy? viewModelAssignmentStrategy = null);

    /// <summary>
    /// Provides a read-only list of the current popup navigation stack.
    /// </summary>
    /// <value>A list of active popup pages.</value>
    IReadOnlyList<IPopupPage> NavigationStack { get; }

    /// <summary>
    /// Event raised when a popup is pushed onto the navigation stack.
    /// This service-level event fires for any popup being added to the stack.
    /// </summary>
    /// <remarks>
    /// Use this event to monitor when popups are added to the stack across the entire application.
    /// For individual popup lifecycle events, use the page-level events on <see cref="IPopupPage"/>.
    /// </remarks>
    event EventHandler<PopupEventArgs>? PopupPushed;

    /// <summary>
    /// Event raised when a popup is popped from the navigation stack.
    /// This service-level event fires for any popup being removed from the stack.
    /// </summary>
    /// <remarks>
    /// Use this event to monitor when popups are removed from the stack across the entire application.
    /// For individual popup lifecycle events, use the page-level events on <see cref="IPopupPage"/>.
    /// </remarks>
    event EventHandler<PopupEventArgs>? PopupPopped;

    /// <summary>
    /// Event raised when the popup navigation stack composition changes.
    /// This fires whenever popups are pushed or popped, providing the updated stack state.
    /// </summary>
    /// <remarks>
    /// Use this event to monitor the overall popup stack state and track navigation history.
    /// The event args contain the current navigation stack after the change.
    /// </remarks>
    event EventHandler<PopupStackChangedEventArgs>? StackChanged;

    /// <summary>
    /// Event raised when a popup is about to open (before it becomes visible).
    /// This service-level event fires for any popup in the application.
    /// </summary>
    /// <remarks>
    /// This event fires before the popup's opening animation begins.
    /// Use this for global popup monitoring. For popup-specific behavior, override
    /// <see cref="PopupPage.OnPopupOpeningAsync"/> or subscribe to the page-level event.
    /// </remarks>
    event EventHandler<PopupEventArgs>? PopupOpening;

    /// <summary>
    /// Event raised after a popup has fully opened (visible and animations complete).
    /// This service-level event fires for any popup in the application.
    /// </summary>
    /// <remarks>
    /// This event fires after the popup's opening animation completes.
    /// Use this for global popup monitoring. For popup-specific behavior, override
    /// <see cref="PopupPage.OnPopupOpenedAsync"/> or subscribe to the page-level event.
    /// </remarks>
    event EventHandler<PopupEventArgs>? PopupOpened;

    /// <summary>
    /// Event raised when a popup is about to close (before it becomes invisible).
    /// This service-level event fires for any popup in the application.
    /// </summary>
    /// <remarks>
    /// This event fires before the popup's closing animation begins.
    /// Use this for global popup monitoring. For popup-specific behavior, override
    /// <see cref="PopupPage.OnPopupClosingAsync"/> or subscribe to the page-level event.
    /// </remarks>
    event EventHandler<PopupEventArgs>? PopupClosing;

    /// <summary>
    /// Event raised after a popup has fully closed (invisible and animations complete).
    /// This service-level event fires for any popup in the application.
    /// </summary>
    /// <remarks>
    /// This event fires after the popup's closing animation completes and it's removed from view.
    /// Use this for global popup monitoring. For popup-specific behavior, override
    /// <see cref="PopupPage.OnPopupClosedAsync"/> or subscribe to the page-level event.
    /// </remarks>
    event EventHandler<PopupEventArgs>? PopupClosed;
}
