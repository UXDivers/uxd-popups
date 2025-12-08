# Popup Class

This page explains the core popup classes in UXDivers Popups: `PopupPage` and `PopupResultPage<T>`.

## PopupPage

`PopupPage` is the base class for all popups in the library. It extends `ContentView` and implements `IPopupPage`, providing the foundation for creating customizable popup overlays.

### Basic Usage

```xml
<uxd:PopupPage
    xmlns:uxd="clr-namespace:UXDivers.Popups.Maui;assembly=UXDivers.Popups.Maui"
    BackgroundColor="{DynamicResource PopupBackdropColor}"
    CloseWhenBackgroundIsClicked="True">

    <!-- Your popup content here -->

</uxd:PopupPage>
```

### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `BackgroundOpacity` | `double?` | null | Opacity of the popup overlay (0-1) |
| `BackgroundColor` | `Color` | Transparent | Background color of the popup overlay |
| `PopupCornerRadius` | `default(CornerRadius)` | 0 | Gets or sets the corner radius of the popup |
| `PopupBackground` | `Brush` | null | Gets or sets the background of the popup |
| `PopupBorderBrush` | `Brush` | null | Gets or sets the border brush of the popup |
| `PopupBorderThickness` | `default(Thickness)` | 0 | Gets or sets the border thickness of the popup |
| `AppearingAnimation` | `IBaseAnimation` | null | Animation when popup appears |
| `DisappearingAnimation` | `IBaseAnimation` | null | Animation when popup disappears |
| `CloseWhenBackgroundIsClicked` | `bool` | false | Auto-close popup when background is tapped |
| `BackgroundClickedCommand` | `ICommand` | null | Command executed when background is clicked |
| `BackgroundClickedCommandParameter` | `object` | null | Parameter for background clicked command |
| `BackgroundInputTransparent` | `bool` | false | Allow taps to pass through the background |
| `DisableWhenIsAnimating` | `bool` | true | Disable interactions during animations |
| `SafeAreaAsPadding` | `SafeAreaAsPadding` | Top\|Right\|Left\|Bottom | Which edges respect safe area insets |
| `PopupContent` | `View` | null | The content of the popup |

---

## PopupResultPage\<T\>

`PopupResultPage<T>` extends `PopupPage` and allows popups to return a typed result when closed. This is useful for forms, confirmations, or any popup that needs to return data.

### Basic Usage

```csharp
// Define a popup that returns a string result
public class MyResultPopup : PopupResultPage<string>
{
    public MyResultPopup()
    {
        // Setup your popup
    }

    private async void OnConfirmClicked()
    {
        // Set the result before closing
        SetResult("User confirmed!");
        await IPopupService.Current.PopAsync(this);
    }
}
```

```csharp
// Show the popup and get the result
var popup = new MyResultPopup();
string? result = await IPopupService.Current.PushAsync(popup);

if (result != null)
{
    Console.WriteLine($"Result: {result}");
}
```

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Result` | `T?` | The result value returned by the popup |

### Methods

| Method | Description |
|--------|-------------|
| `SetResult(T? result)` | Sets the popup result before closing |

---

## Popup Lifecycle

Popups have a well-defined lifecycle with events and virtual methods that you can override to hook into different stages.

### Lifecycle Events

| Event | When it Fires |
|-------|---------------|
| `PopupOpening` | Before the popup becomes visible (before animation) |
| `PopupOpened` | After the popup is fully visible (after animation) |
| `PopupClosing` | Before the popup starts closing (before animation) |
| `PopupClosed` | After the popup is fully closed (after animation) |
| `PopupBackgroundClicked` | When the background overlay is tapped |

### Virtual Methods

Override these methods in your custom popup to add behavior at lifecycle stages:

```csharp
public class MyCustomPopup : PopupPage
{
    public override void OnAppearing()
    {
        base.OnAppearing();
        // Called when the popup is appearing
        Console.WriteLine("Popup is appearing!");
    }

    public override void OnDisappearing()
    {
        base.OnDisappearing();
        // Called when the popup is disappearing
        Console.WriteLine("Popup is disappearing!");
    }

    public override void OnNavigatedTo(IReadOnlyDictionary<string, object?> parameters)
    {
        base.OnNavigatedTo(parameters);
        // Called when navigation parameters are passed to the popup
        if (parameters.TryGetValue("userId", out var userId))
        {
            Console.WriteLine($"Received userId: {userId}");
        }
    }

    public override async Task OnPopupOpeningAsync(PopupEventArgs e)
    {
        await base.OnPopupOpeningAsync(e);
        // Custom logic before popup opens
    }

    public override async Task OnPopupOpenedAsync(PopupEventArgs e)
    {
        await base.OnPopupOpenedAsync(e);
        // Custom logic after popup opens
    }

    public override async Task OnPopupClosingAsync(PopupEventArgs e)
    {
        await base.OnPopupClosingAsync(e);
        // Custom logic before popup closes
    }

    public override async Task OnPopupClosedAsync(PopupEventArgs e)
    {
        await base.OnPopupClosedAsync(e);
        // Custom logic after popup closes
    }
}
```

### Subscribing to Events

You can also subscribe to lifecycle events externally:

```csharp
var popup = new Toast { Title = "Hello!" };

popup.PopupOpening += (sender, args) =>
{
    Console.WriteLine("Popup is opening!");
};

popup.PopupClosed += (sender, args) =>
{
    Console.WriteLine("Popup was closed!");
};

await IPopupService.Current.PushAsync(popup);
```

---

## SafeArea Property

The `SafeAreaAsPadding` property controls which edges of the popup respect device safe area insets (notches, home indicators, etc.).

### SafeAreaAsPadding Values

| Value | Description |
|-------|-------------|
| `None` | No safe area insets are applied |
| `Top` | Apply safe area insets to the top edge |
| `Left` | Apply safe area insets to the left edge |
| `Right` | Apply safe area insets to the right edge |
| `Bottom` | Apply safe area insets to the bottom edge |
| `All` | Apply safe area insets to all edges |

The default value is `Top | Right | Left`, which applies safe area to top and sides but not bottom (useful for popups anchored to the bottom).

### Usage Examples

```xml
<!-- Full safe area on all edges -->
<uxd:PopupPage SafeAreaAsPadding="All">
    <!-- Content -->
</uxd:PopupPage>

<!-- Only top safe area (for top-anchored popups like toasts) -->
<uxd:PopupPage SafeAreaAsPadding="Top">
    <!-- Content -->
</uxd:PopupPage>

<!-- Bottom sheet style (no safe area to allow content to extend to edges) -->
<uxd:PopupPage SafeAreaAsPadding="None">
    <!-- Content -->
</uxd:PopupPage>

<!-- Combine multiple edges -->
<uxd:PopupPage SafeAreaAsPadding="Top,Bottom">
    <!-- Content -->
</uxd:PopupPage>
```

### When to Use Different Values

| Popup Type | Recommended SafeAreaAsPadding |
|------------|-------------------------------|
| Toast (top) | `Top` or `Top,Left,Right` |
| Bottom Sheet | `Bottom` or `All` |
| Centered Dialog | `All` or default |
| Fullscreen Modal | `All` |

---

## Background Customization

The popup background (overlay) can be customized in several ways to control its appearance and behavior.

### Background Opacity

Control the darkness of the overlay:

```xml
<!-- Semi-transparent dark overlay -->
<uxd:PopupPage
    BackgroundColor="#000000"
    BackgroundOpacity="0.5">
    <!-- Content -->
</uxd:PopupPage>

<!-- Light overlay -->
<uxd:PopupPage
    BackgroundColor="#FFFFFF"
    BackgroundOpacity="0.3">
    <!-- Content -->
</uxd:PopupPage>

<!-- Using theme colors -->
<uxd:PopupPage
    BackgroundColor="{DynamicResource PopupBackdropColor}">
    <!-- Content -->
</uxd:PopupPage>
```

The `PopupBackdropColor` in the default theme is `#AA000000` (black with ~67% opacity).

### Background Interaction

Control how the background responds to user interaction:

```xml
<!-- Close popup when background is tapped -->
<uxd:PopupPage CloseWhenBackgroundIsClicked="True">
    <!-- Content -->
</uxd:PopupPage>

<!-- Execute command when background is tapped -->
<uxd:PopupPage
    BackgroundClickedCommand="{Binding OnBackgroundTappedCommand}"
    BackgroundClickedCommandParameter="background_tap">
    <!-- Content -->
</uxd:PopupPage>

<!-- Allow taps to pass through to underlying content -->
<uxd:PopupPage BackgroundInputTransparent="True">
    <!-- Content -->
</uxd:PopupPage>
```

### Combining Options

```xml
<uxd:PopupPage
    BackgroundColor="#000000"
    BackgroundOpacity="0.6"
    CloseWhenBackgroundIsClicked="True"
    BackgroundClickedCommand="{Binding LogDismissCommand}">
    <!-- Content -->
</uxd:PopupPage>
```

### Transparent Background

For popups that should appear without an overlay (like floating notifications):

```xml
<uxd:PopupPage
    BackgroundColor="Transparent"
    BackgroundInputTransparent="True"
    CloseWhenBackgroundIsClicked="False">
    <!-- Content -->
</uxd:PopupPage>
```

---

## Animation During Interaction

The `DisableWhenIsAnimating` property (default: `true`) prevents user interaction during popup animations. This ensures smooth animations without accidental taps.

```xml
<!-- Allow interaction during animations (not recommended) -->
<uxd:PopupPage DisableWhenIsAnimating="False">
    <!-- Content -->
</uxd:PopupPage>
```

You can also programmatically control interaction:

```csharp
// Disable interaction
popup.SetInteractionEnabled(false);

// Re-enable interaction
popup.SetInteractionEnabled(true);
```

---

## Service-Level Events

`IPopupService` also provides events to monitor popup activity globally:

```csharp
// Monitor all popup activity
IPopupService.Current.PopupPushed += (sender, args) =>
{
    Console.WriteLine($"Popup pushed: {args.PopupPage.GetType().Name}");
};

IPopupService.Current.PopupPopped += (sender, args) =>
{
    Console.WriteLine($"Popup popped: {args.PopupPage.GetType().Name}");
};

IPopupService.Current.StackChanged += (sender, args) =>
{
    Console.WriteLine($"Stack size: {args.Stack.Count}");
};
```

---

## Navigation Stack

Access the current popup stack through `IPopupService`:

```csharp
// Get all active popups
IReadOnlyList<IPopupPage> stack = IPopupService.Current.NavigationStack;

// Check if any popups are open
bool hasPopups = stack.Count > 0;

// Get the topmost popup
IPopupPage? topPopup = stack.LastOrDefault();
```
