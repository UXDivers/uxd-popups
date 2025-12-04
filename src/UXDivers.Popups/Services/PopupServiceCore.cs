namespace UXDivers.Popups.Services;

/// <summary>
/// Core popup service implementation that provides framework-agnostic popup management.
/// </summary>
public class PopupServiceCore : IPopupService
{
    private static PopupServiceCore? _instance;
    private static readonly object _lock = new();
    private readonly IReadOnlyDictionary<string, object?> _emptyParameters = new Dictionary<string, object?>();

    /// <summary>
    /// Gets the singleton instance of the core popup service.
    /// </summary>
    public static PopupServiceCore Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _ = _instance ??= new PopupServiceCore();
                }
            }
            return _instance;
        }
    }

    private readonly List<PopupStackItemCore> _popupStack = new();
    private INativePopupManager? _nativePopupManager;
    private IUIThreadDispatcher? _uiThreadDispatcher;
    private IViewModelAssignmentStrategy? _viewModelAssignmentStrategy;

    /// <summary>
    /// Private constructor to enforce singleton pattern.
    /// </summary>
    private PopupServiceCore()
    {
    }

    /// <inheritdoc/>
    public bool IsInitialized => _nativePopupManager != null && _uiThreadDispatcher != null;

    /// <inheritdoc/>
    public IReadOnlyList<IPopupPage> NavigationStack => 
        _popupStack.Select(item => item.PopupPage).ToList().AsReadOnly();

    /// <inheritdoc/>
    public event EventHandler<PopupEventArgs>? PopupPushed;

    /// <inheritdoc/>
    public event EventHandler<PopupEventArgs>? PopupPopped;

    /// <inheritdoc/>
    public event EventHandler<PopupStackChangedEventArgs>? StackChanged;

    /// <inheritdoc/>
    public event EventHandler<PopupEventArgs>? PopupOpening;

    /// <inheritdoc/>
    public event EventHandler<PopupEventArgs>? PopupOpened;

    /// <inheritdoc/>
    public event EventHandler<PopupEventArgs>? PopupClosing;

    /// <inheritdoc/>
    public event EventHandler<PopupEventArgs>? PopupClosed;

    /// <inheritdoc/>
    public void Initialize(
        INativePopupManager nativePopupManager, 
        IUIThreadDispatcher uiThreadDispatcher, 
        IViewModelAssignmentStrategy? viewModelAssignmentStrategy = null)
    {
        _nativePopupManager = nativePopupManager ?? throw new ArgumentNullException(nameof(nativePopupManager));
        _uiThreadDispatcher = uiThreadDispatcher ?? throw new ArgumentNullException(nameof(uiThreadDispatcher));
        _viewModelAssignmentStrategy = viewModelAssignmentStrategy ?? NoViewModelAssignmentStrategy.Instance;
    }


     /// <inheritdoc />
    public Task PushAsync<TPopup>(Dictionary<string, object?>? parameters = null, bool waitUntilClosed = true) 
        where TPopup : class, IPopupPage
    {
        var popup = PopupRegistryService.Instance.CreatePopupInstance<TPopup>();
        if (popup == null)
        {
            throw new InvalidOperationException($"Could not create popup instance of type {typeof(TPopup).Name}");
        }

        return PushAsync(popup, parameters, waitUntilClosed);
    }
    
    /// <inheritdoc />
    public Task<TResult?> PushAsync<TPopupResult, TResult>(Dictionary<string, object?>? parameters = null) 
        where TPopupResult : class, IPopupResultPage<TResult>
    {
        var popup = PopupRegistryService.Instance.CreatePopupInstance<TPopupResult>() as IPopupResultPage<TResult>;
        if (popup == null)
        {
            throw new InvalidOperationException($"Could not create popup instance of type {typeof(TPopupResult).Name}");
        }

        return PushAsync<TResult>(popup, parameters);
    }

    /// <inheritdoc/>
    public Task PushAsync(IPopupPage popupPage, Dictionary<string, object?>? parameters = null, bool waitUntilClosed = true)
    {
        CheckInitialized();

        if (popupPage == null)
        {
            throw new ArgumentNullException(nameof(popupPage));
        }

        var stackItem = new PopupStackItemCore
        {
            PopupPage = popupPage,
            TaskSource = new TaskCompletionSource()
        };

        TaskCompletionSource? waitUntilOpenedTCS = !waitUntilClosed ? new TaskCompletionSource() : null;

        IReadOnlyDictionary<string, object?> navParameters = parameters ?? _emptyParameters;

        ShowPopup(stackItem, navParameters, waitUntilOpenedTCS);

        if (waitUntilOpenedTCS != null)
        {
            return waitUntilOpenedTCS.Task;
        }

        return stackItem.TaskSource.Task;
    }

    /// <inheritdoc/>
    public Task<T?> PushAsync<T>(IPopupResultPage<T> popupPage, Dictionary<string, object?>? parameters = null)
    {
        CheckInitialized();

        if (popupPage == null)
        {
            throw new ArgumentNullException(nameof(popupPage));
        }

        var stackItem = new PopupResultStackItemCore<T>
        {
            PopupPage = popupPage,
            TaskSource = new TaskCompletionSource<T?>()
        };

        IReadOnlyDictionary<string, object?> navParameters = parameters ?? _emptyParameters;

        ShowPopup(stackItem, navParameters);

        return stackItem.TaskSource.Task;
    }

    /// <inheritdoc/>
    public async Task PopAsync(IPopupPage? popupPage = null)
    {
        CheckInitialized();

        var stackItem = await ClosePopupAsync(popupPage);
        if (stackItem != null)
        {
            stackItem.SetResult();
        }
    }

    /// <inheritdoc/>
    public async Task PopAllAsync()
    {
        CheckInitialized();

        while (_popupStack.Count > 0)
        {
            await PopAsync();
        }
    }

    /// <summary>
    /// Shows a popup and adds it to the stack.
    /// </summary>
    /// <param name="stackItem">The popup stack item to display.</param>
    /// <param name="parameters">Navigation parameters.</param>
    private void ShowPopup(PopupStackItemCore stackItem, IReadOnlyDictionary<string, object?> parameters, TaskCompletionSource? waitUntilOpenedTCS = null)
    {
        // Assign ViewModel if service provider is available
        AssignViewModel(stackItem.PopupPage);

        _uiThreadDispatcher!.Dispatch(async () =>
        {
            // Pass navigation parameters to the popup
            stackItem.PopupPage.OnNavigatedTo(parameters);

            // Pass navigation parameters to the viewmodel
            if (_viewModelAssignmentStrategy?.SupportsViewModelAssignment == true)
            {
                try
                {
                    await _viewModelAssignmentStrategy.SetParameters(stackItem.PopupPage, parameters);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in OnPopupNavigatedAsync: {ex}");
                }
            }

            // Raise PopupOpening event before showing the popup
            PopupOpening?.Invoke(this, new PopupEventArgs(stackItem.PopupPage));

            // Call page-level OnPopupOpeningAsync
            await stackItem.PopupPage.OnPopupOpeningAsync(new PopupEventArgs(stackItem.PopupPage));

            // Show the popup using the native manager
            var nativePopup = await _nativePopupManager!.ShowNativeViewAsync(stackItem.PopupPage);
            stackItem.NativePopup = nativePopup;

            // Add to stack
            _popupStack.Add(stackItem);

            // Call appearing callback
            stackItem.PopupPage.OnAppearing();

            // Run appearing animation if available
            if (stackItem.PopupPage.AppearingAnimation != null)
            {
                //Prepare animation
                stackItem.PopupPage.AppearingAnimation.PrepareAnimation(stackItem.PopupPage);

                // Disable interaction during animation if requested
                if (stackItem.PopupPage.DisableWhenIsAnimating)
                {
                    stackItem.PopupPage.SetInteractionEnabled(false);
                }
                
                try
                {
                    await stackItem.PopupPage.AppearingAnimation.RunAnimation(stackItem.PopupPage);
                }
                catch (OperationCanceledException)
                {
                    // The animation was cancelled
                }

                // Re-enable interaction after animation if it was disabled
                if (stackItem.PopupPage.DisableWhenIsAnimating)
                {
                    stackItem.PopupPage.SetInteractionEnabled(true);
                }
            }

            // If the popup was requested to wait until opened, set the result now
            _ = waitUntilOpenedTCS?.TrySetResult();

            // Raise PopupOpened event after animations complete
            PopupOpened?.Invoke(this, new PopupEventArgs(stackItem.PopupPage));

            // Call page-level OnPopupOpenedAsync
            await stackItem.PopupPage.OnPopupOpenedAsync(new PopupEventArgs(stackItem.PopupPage));

            // Raise stack management events
            PopupPushed?.Invoke(this, new PopupEventArgs(stackItem.PopupPage));
            StackChanged?.Invoke(this, new PopupStackChangedEventArgs(NavigationStack));
        });
    }

    /// <summary>
    /// Closes a popup and removes it from the stack.
    /// </summary>
    /// <param name="popupPage">The specific popup to close, or null for the top popup.</param>
    /// <returns>The closed stack item.</returns>
    private async Task<PopupStackItemCore?> ClosePopupAsync(IPopupPage? popupPage = null)
    {
        if (_popupStack.Count == 0)
        {
            return null;
        }

        PopupStackItemCore stackItem;

        if (popupPage == null)
        {
            // Pop the top popup
            stackItem = _popupStack.Last();
        }
        else
        {
            // Find the specific popup
            stackItem = _popupStack.FirstOrDefault(item => item.PopupPage == popupPage)!;
            if (stackItem == null)
            {
                return null;
            }
        }

        await _uiThreadDispatcher!.DispatchAsync(async () =>
        {
            // Raise PopupClosing event before closing the popup
            PopupClosing?.Invoke(this, new PopupEventArgs(stackItem.PopupPage));

            // Call page-level OnPopupClosingAsync
            await stackItem.PopupPage.OnPopupClosingAsync(new PopupEventArgs(stackItem.PopupPage));

            // Call disappearing callback
            stackItem.PopupPage.OnDisappearing();

            // Run disappearing animation if available
            if (stackItem.PopupPage.DisappearingAnimation != null)
            {
                // Prepare animation
                stackItem.PopupPage.DisappearingAnimation.PrepareAnimation(stackItem.PopupPage);

                // Disable interaction during animation if requested
                if (stackItem.PopupPage.DisableWhenIsAnimating)
                {
                    stackItem.PopupPage.SetInteractionEnabled(false);
                }
                
                try
                {
                    await stackItem.PopupPage.DisappearingAnimation.RunAnimation(stackItem.PopupPage);
                }
                catch (OperationCanceledException)
                {
                    // The animation was cancelled
                }


                // Re-enable interaction after animation if it was disabled
                if (stackItem.PopupPage.DisableWhenIsAnimating)
                {
                    stackItem.PopupPage.SetInteractionEnabled(true);
                }
            }

            // Close the native popup
            if (stackItem.NativePopup != null)
            {
                await _nativePopupManager!.CloseNativeViewAsync(stackItem.NativePopup);
            }

            // Remove from stack
            _popupStack.Remove(stackItem);

            // Raise PopupClosed event after popup is fully closed
            PopupClosed?.Invoke(this, new PopupEventArgs(stackItem.PopupPage));

            // Call page-level OnPopupClosedAsync
            await stackItem.PopupPage.OnPopupClosedAsync(new PopupEventArgs(stackItem.PopupPage));

            // Raise stack management events
            PopupPopped?.Invoke(this, new PopupEventArgs(stackItem.PopupPage));
            StackChanged?.Invoke(this, new PopupStackChangedEventArgs(NavigationStack));
        });

        return stackItem;
    }

    /// <summary>
    /// Assigns a ViewModel to the popup page if a service provider is available and the popup page does not already have a ViewModel assigned.
    /// </summary>
    /// <param name="popupPage">The popup page to assign a ViewModel to.</param>
    private void AssignViewModel(IPopupPage popupPage)
    {
        if (_viewModelAssignmentStrategy?.SupportsViewModelAssignment != true)
        {
            return;
        }

        try
        {
            if (_viewModelAssignmentStrategy.HasViewModel(popupPage))
            {
                return;
            }

            var viewModel = PopupRegistryService.Instance.CreateViewModel(popupPage);
            if (viewModel != null)
            {
                _viewModelAssignmentStrategy.TryAssignViewModel(popupPage, viewModel);
            }
        }
        catch
        {
            // Ignore ViewModel assignment failures
        }
    }

    /// <summary>
    /// Checks if the service has been initialized.
    /// </summary>
    private void CheckInitialized()
    {
        if (!IsInitialized)
        {
            throw new InvalidOperationException("UXDPopupServiceCore has not been initialized. Call Initialize() first.");
        }
    }
}
