using UXDivers.Popups.Services;

namespace UXDivers.Popups.Maui;

/// <summary>
/// Implementation of the native popup manager for handling platform-specific popup logic.
/// </summary>
internal partial class NativePopupManager : INativePopupManager
{
    /// <summary>
    /// Retrieves the <see cref="IMauiContext"/> for the current application.
    /// </summary>
    /// <returns>The <see cref="IMauiContext"/> associated with the main application window.</returns>
    /// <exception cref="NullReferenceException">Thrown if the application or its components are null.</exception>
    /// <exception cref="IndexOutOfRangeException">Thrown if no windows are created in the application.</exception>
    protected IMauiContext GetMauiContext()
    {
        // Ensure the application is not null
        if (Application.Current == null)
        {
            throw new NullReferenceException("Application.Current is null");
        }

        // Ensure there is at least one window created
        if (Application.Current.Windows.Count == 0)
        {
            throw new IndexOutOfRangeException("No application windows have been created yet");
        }

        // Ensure the main window handler is not null
        if (Application.Current.Windows[0].Handler == null)
        {
            throw new NullReferenceException("Main window handler is null");
        }

        // Retrieve the MauiContext from the main window handler
        var mauiContext = Application.Current?.Windows[0].Handler.MauiContext;

        // Ensure the MauiContext is not null
        if (mauiContext == null)
        {
            throw new NullReferenceException("MauiContext is null");
        }

        return mauiContext;
    }
    
    public virtual Task<object> ShowNativeViewAsync(IPopupPage popup)
    {
        if (popup is PopupPage popupPage)
        {
            if (Application.Current == null || Application.Current.Windows.Count == 0)
            {
                throw new NullReferenceException("Application.Current or its windows are not properly initialized");
            }

            popupPage.Parent = Application.Current?.Windows[0];

            return ShowNativeViewAsync(popupPage);
        }
        
        throw new ArgumentException("The provided popup does not inherit from PopupPage", nameof(popup));
    }
}
