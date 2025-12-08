using System.Windows.Input;
using UXDivers.Popups.Services;
using System.Runtime.CompilerServices;

namespace UXDivers.Popups.Maui
{
    /// <summary>
    /// Represents a popup page with customizable animations, background behavior, and commands.
    /// </summary>
    [ContentProperty(nameof(PopupContent))]
    public class PopupPage : ContentView, IPopupPage
    {
        /// <summary>
        /// Bindable property for the background opacity.
        /// </summary>
        public static readonly BindableProperty BackgroundOpacityProperty = BindableProperty.Create(
            nameof(BackgroundOpacity),
            typeof(double?),
            typeof(PopupPage),
            null,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                if (bindable is PopupPage popupPage)
                {
                    popupPage.UpdateBackgroundColorOpacity();
                }
            });

        /// <summary>
        /// Gets or sets the opacity of the popup overlay.
        /// </summary>
        public double? BackgroundOpacity
        {
            get => (double?)GetValue(BackgroundOpacityProperty);
            set => SetValue(BackgroundOpacityProperty, value);
        }

        /// <summary>
        /// Bindable property for the animation to run when the popup is appearing.
        /// </summary>
        public static readonly BindableProperty AppearingAnimationProperty = BindableProperty.Create(
            nameof(AppearingAnimation),
            typeof(IBaseAnimation),
            typeof(PopupPage),
            null);

        /// <summary>
        /// Gets or sets the animation to use when the popup appears.
        /// </summary>
        public IBaseAnimation AppearingAnimation
        {
            get => (IBaseAnimation)GetValue(AppearingAnimationProperty);
            set => SetValue(AppearingAnimationProperty, value);
        }

        /// <summary>
        /// Bindable property for the animation to run when the popup is disappearing.
        /// </summary>
        public static readonly BindableProperty DisappearingAnimationProperty = BindableProperty.Create(
            nameof(DisappearingAnimation),
            typeof(IBaseAnimation),
            typeof(PopupPage),
            null);

        /// <summary>
        /// Gets or sets the animation to use when the popup disappears.
        /// </summary>
        public IBaseAnimation DisappearingAnimation
        {
            get => (IBaseAnimation)GetValue(DisappearingAnimationProperty);
            set => SetValue(DisappearingAnimationProperty, value);
        }

        /// <summary>
        /// Bindable property indicating whether the popup should close when the background is clicked.
        /// </summary>
        public static readonly BindableProperty CloseWhenBackgroundIsClickedProperty = BindableProperty.Create(
            nameof(CloseWhenBackgroundIsClicked),
            typeof(bool),
            typeof(PopupPage),
            false);

        /// <summary>
        /// Gets a value indicating whether the popup should close when the background is clicked.
        /// </summary>
        public bool CloseWhenBackgroundIsClicked
        {
            get => (bool)GetValue(CloseWhenBackgroundIsClickedProperty);
            set => SetValue(CloseWhenBackgroundIsClickedProperty, value);
        }

        /// <summary>
        /// Bindable property for the command to execute when the background is clicked.
        /// </summary>
        public static readonly BindableProperty BackgroundClickedCommandProperty = BindableProperty.Create(
            nameof(BackgroundClickedCommand),
            typeof(ICommand),
            typeof(PopupPage),
            null);

        /// <summary>
        /// Gets the command to execute when the background is clicked.
        /// </summary>
        public ICommand BackgroundClickedCommand
        {
            get => (ICommand)GetValue(BackgroundClickedCommandProperty);
            set => SetValue(BackgroundClickedCommandProperty, value);
        }

        /// <summary>
        /// Bindable property for the parameter to pass to the <see cref="BackgroundClickedCommand"/>.
        /// </summary>
        public static readonly BindableProperty BackgroundClickedCommandParameterProperty = BindableProperty.Create(
            nameof(BackgroundClickedCommandParameter),
            typeof(object),
            typeof(PopupPage),
            null);

        /// <summary>
        /// Gets the parameter to pass to the <see cref="BackgroundClickedCommand"/>.
        /// </summary>
        public object BackgroundClickedCommandParameter
        {
            get => GetValue(BackgroundClickedCommandParameterProperty);
            set => SetValue(BackgroundClickedCommandParameterProperty, value);
        }

        /// <summary>
        /// Bindable property indicating whether the background is input transparent.
        /// </summary>
        public static readonly BindableProperty BackgroundInputTransparentProperty = BindableProperty.Create(
            nameof(BackgroundInputTransparent),
            typeof(bool),
            typeof(PopupPage),
            false);

        /// <summary>
        /// Gets a value indicating whether the background is input transparent.
        /// </summary>
        public bool BackgroundInputTransparent
        {
            get => (bool)GetValue(BackgroundInputTransparentProperty);
            set => SetValue(BackgroundInputTransparentProperty, value);
        }

        /// <summary>
        /// Bindable property indicating whether the popup should disable interactions while animating.
        /// </summary>
        public static readonly BindableProperty DisableWhenIsAnimatingProperty = BindableProperty.Create(
            nameof(DisableWhenIsAnimating),
            typeof(bool),
            typeof(PopupPage),
            true);

        /// <summary>
        /// Gets a value indicating whether the popup should disable interactions while animating.
        /// </summary>
        public bool DisableWhenIsAnimating
        {
            get => (bool)GetValue(DisableWhenIsAnimatingProperty);
            set => SetValue(DisableWhenIsAnimatingProperty, value);
        }

        /// <summary>
        /// Bindable property for specifying which edges should respect safe area insets.
        /// </summary>
        public static readonly BindableProperty SafeAreaAsPaddingProperty = BindableProperty.Create(
            nameof(SafeAreaAsPadding),
            typeof(SafeAreaAsPadding),
            typeof(PopupPage),
            SafeAreaAsPadding.Top | SafeAreaAsPadding.Right | SafeAreaAsPadding.Left);

        /// <summary>
        /// Gets or sets which edges of the popup should respect safe area insets.
        /// Default is All, which applies safe area insets to all edges.
        /// </summary>
        public SafeAreaAsPadding SafeAreaAsPadding
        {
            get => (SafeAreaAsPadding)GetValue(SafeAreaAsPaddingProperty);
            set => SetValue(SafeAreaAsPaddingProperty, value);
        }

        /// <summary>
        /// Bindable property for specifying popup corner radius.
        /// </summary>
        public static readonly BindableProperty PopupCornerRadiusProperty = BindableProperty.Create(
            nameof(PopupCornerRadius),
            typeof(CornerRadius),
            typeof(PopupPage),
            default(CornerRadius));

        /// <summary>
        /// Gets or sets the corner radius of the popup.
        /// Default <see cref="CornerRadius" default />
        /// </summary>
        public CornerRadius PopupCornerRadius
        {
            get => (CornerRadius)GetValue(PopupCornerRadiusProperty);
            set => SetValue(PopupCornerRadiusProperty, value);
        }

        /// <summary>
        /// Bindable property for specifying popup background.
        /// </summary>
        public static readonly BindableProperty PopupBackgroundProperty = BindableProperty.Create(
            nameof(PopupBackground),
            typeof(Brush),
            typeof(PopupPage),
            null);

        /// <summary>
        /// Gets or sets the background of the popup.
        /// Default is null.
        /// </summary>
        public Brush? PopupBackground
        {
            get => (Brush?)GetValue(PopupBackgroundProperty);
            set => SetValue(PopupBackgroundProperty, value);
        }

        /// <summary>
        /// Bindable property for specifying popup border brush.
        /// </summary>
        public static readonly BindableProperty PopupBorderBrushProperty = BindableProperty.Create(
            nameof(PopupBorderBrush),
            typeof(Brush),
            typeof(PopupPage),
            null);

        /// <summary>
        /// Gets or sets the border brush of the popup.
        /// Default is null.
        /// </summary>
        public Brush? PopupBorderBrush
        {
            get => (Brush?)GetValue(PopupBorderBrushProperty);
            set => SetValue(PopupBorderBrushProperty, value);
        }

        /// <summary>
        /// Bindable property for specifying popup border brush.
        /// Default <see cref="Thickness" default />
        /// </summary>
        public static readonly BindableProperty PopupBorderThicknessProperty = BindableProperty.Create(
            nameof(PopupBorderThickness),
            typeof(Thickness),
            typeof(PopupPage),
            default(Thickness));

        /// <summary>
        /// Gets or sets the border thickness of the popup.
        /// </summary>
        public Thickness PopupBorderThickness
        {
            get => (Thickness)GetValue(PopupBorderThicknessProperty);
            set => SetValue(PopupBorderThicknessProperty, value);
        }

        /// <summary>
        /// Gets or sets the content of the popup page.
        /// </summary>
        public View? PopupContent
        {
            get => Content;
            set => Content = value;
        }

        /// <summary>
        /// ONLY FOR INTERNAL USE, Gets the content of the popup page, taking into account the ControlTemplate if applied.
        /// </summary>
        public View? ActualContent => (this as IVisualTreeElement)?.GetVisualChildren().FirstOrDefault() as View;

        public PopupPage()
        {
            //Disable safe area handling by maui, we handle it ourselves based on SafeAreaInsets property.
#if NET10_0_OR_GREATER
            SafeAreaEdges = SafeAreaEdges.None;
#endif
        }

        /// <summary>
        /// Called when the popup is appearing.
        /// </summary>
        public virtual void OnAppearing()
        {
            // Override in derived classes for custom appearing logic
        }

        /// <summary>
        /// Called when the popup is disappearing.
        /// </summary>
        public virtual void OnDisappearing()
        {
            // Override in derived classes for custom disappearing logic
        }

        /// <summary>
        /// Called when the popup receives navigation parameters.
        /// </summary>
        /// <param name="parameters">The navigation parameters.</param>
        public virtual void OnNavigatedTo(IReadOnlyDictionary<string, object?> parameters)
        {
            // Override in derived classes for custom navigation logic
        }

        // Popup lifecycle events with proper PopupEventArgs
        public event EventHandler<PopupEventArgs>? PopupOpening;
        public event EventHandler<PopupEventArgs>? PopupOpened;
        public event EventHandler<PopupEventArgs>? PopupClosing;
        public event EventHandler<PopupEventArgs>? PopupClosed;
        public event EventHandler<PopupEventArgs>? PopupBackgroundClicked;

        protected override void OnPropertyChanging([CallerMemberName] string propertyName = "")
        {
            if (propertyName == nameof(BackgroundColor) 
                || propertyName == nameof(Background) && Background is SolidColorBrush)
            {
                UpdateBackgroundColorOpacity();
            }

            base.OnPropertyChanging(propertyName);
        }

        /// <summary>
        /// Handles the background tap event.
        /// Executes the background clicked command and optionally closes the popup.
        /// </summary>
        internal async Task OnBackgroundTapped()
        {
            await OnPopupBackgroundClickedAsync(new PopupEventArgs(this));
            BackgroundClickedCommand?.Execute(BackgroundClickedCommandParameter);

            if (CloseWhenBackgroundIsClicked)
            {
                await IPopupService.Current.PopAsync(this);
            }
        }

        // Popup lifecycle event invokers and virtuals
        /// <inheritdoc/>
        public virtual async Task OnPopupOpeningAsync(PopupEventArgs e)
        {
            PopupOpening?.Invoke(this, e);
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual async Task OnPopupOpenedAsync(PopupEventArgs e)
        {
            PopupOpened?.Invoke(this, e);
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual async Task OnPopupClosingAsync(PopupEventArgs e)
        {
            PopupClosing?.Invoke(this, e);
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual async Task OnPopupClosedAsync(PopupEventArgs e)
        {
            PopupClosed?.Invoke(this, e);
            await Task.CompletedTask;
        }

        protected internal virtual async Task OnPopupBackgroundClickedAsync(PopupEventArgs e)
        {
            PopupBackgroundClicked?.Invoke(this, e);
            await Task.CompletedTask;
        }

        /// <summary>
        /// Sets the interaction enabled state for the popup.
        /// </summary>
        /// <param name="enabled">True to enable interaction, false to disable.</param>
        public virtual void SetInteractionEnabled(bool enabled)
        {
            InputTransparent = !enabled;
        }

        private void UpdateBackgroundColorOpacity()
        {
            if (BackgroundOpacity == null || BackgroundOpacity > 1 || BackgroundOpacity < 0)
            {
                return;
            }

            if (Background is SolidColorBrush solidColorBrush && solidColorBrush.Color != null)
            {
                solidColorBrush.Color = solidColorBrush.Color.WithAlpha((float)BackgroundOpacity);
            }
            else if (BackgroundColor != null)
            {
                BackgroundColor = BackgroundColor.WithAlpha((float)BackgroundOpacity);
            }
        }
    }
}
