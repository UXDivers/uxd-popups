using System.Windows.Input;

namespace UXDivers.Popups;

/// <summary>
/// Interface representing a popup page with customizable animations, content, and behavior.
/// </summary>
public interface IPopupPage
{
    /// <summary>
    /// Gets or sets the animation to use when the popup appears.
    /// </summary>
    IBaseAnimation AppearingAnimation { get; set; }

    /// <summary>
    /// Gets or sets the animation to use when the popup disappears.
    /// </summary>
    IBaseAnimation DisappearingAnimation { get; set; }

    /// <summary>
    /// Gets a value indicating whether the popup should close when the background is clicked.
    /// </summary>
    bool CloseWhenBackgroundIsClicked { get; }

    /// <summary>
    /// Gets a value indicating whether the background is input transparent.
    /// </summary>
    bool BackgroundInputTransparent { get; }

    /// <summary>
    /// Gets a value indicating whether the popup should disable interactions while animating.
    /// </summary>
    bool DisableWhenIsAnimating { get; }

    /// <summary>
    /// Gets the command to execute when the background is clicked.
    /// </summary>
    ICommand BackgroundClickedCommand { get; }

    /// <summary>
    /// Gets the parameter to pass to the <see cref="BackgroundClickedCommand"/>.
    /// </summary>
    object BackgroundClickedCommandParameter { get; }

    /// <summary>
    /// Called when the popup is appearing.
    /// </summary>
    void OnAppearing();

    /// <summary>
    /// Called when the popup is disappearing.
    /// </summary>
    void OnDisappearing();

    /// <summary>
    /// Called when the popup receives navigation parameters.
    /// </summary>
    /// <param name="parameters">The navigation parameters.</param>
    void OnNavigatedTo(IReadOnlyDictionary<string, object?> parameters);

    /// <summary>
    /// Occurs when the popup is about to open.
    /// </summary>
    event EventHandler<PopupEventArgs>? PopupOpening;
    
    /// <summary>
    /// Occurs after the popup has opened.
    /// </summary>
    event EventHandler<PopupEventArgs>? PopupOpened;
    
    /// <summary>
    /// Occurs when the popup is about to close.
    /// </summary>
    event EventHandler<PopupEventArgs>? PopupClosing;
    
    /// <summary>
    /// Occurs after the popup has closed.
    /// </summary>
    event EventHandler<PopupEventArgs>? PopupClosed;
    
    /// <summary>
    /// Occurs when the popup background is clicked.
    /// </summary>
    event EventHandler<PopupEventArgs>? PopupBackgroundClicked;

    /// <summary>
    /// Called when the popup is about to open.
    /// This method is invoked by the popup service before the popup becomes visible.
    /// </summary>
    /// <param name="e">The event arguments containing popup information.</param>
    /// <returns>A task that completes when the opening logic finishes.</returns>
    Task OnPopupOpeningAsync(PopupEventArgs e);

    /// <summary>
    /// Called when the popup has fully opened.
    /// This method is invoked by the popup service after opening animations complete.
    /// </summary>
    /// <param name="e">The event arguments containing popup information.</param>
    /// <returns>A task that completes when the opened logic finishes.</returns>
    Task OnPopupOpenedAsync(PopupEventArgs e);

    /// <summary>
    /// Called when the popup is about to close.
    /// This method is invoked by the popup service before the popup becomes invisible.
    /// </summary>
    /// <param name="e">The event arguments containing popup information.</param>
    /// <returns>A task that completes when the closing logic finishes.</returns>
    Task OnPopupClosingAsync(PopupEventArgs e);

    /// <summary>
    /// Called when the popup has fully closed.
    /// This method is invoked by the popup service after closing animations complete.
    /// </summary>
    /// <param name="e">The event arguments containing popup information.</param>
    /// <returns>A task that completes when the closed logic finishes.</returns>
    Task OnPopupClosedAsync(PopupEventArgs e);

    /// <summary>
    /// Sets the interaction enabled state for the popup.
    /// </summary>
    /// <param name="enabled">True to enable interaction, false to disable.</param>
    /// <remarks>
    /// Platform implementations should handle this appropriately.
    /// </remarks>
    void SetInteractionEnabled(bool enabled);
}
