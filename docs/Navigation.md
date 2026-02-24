# Navigation

This page explains how to navigate popups using `IPopupService`, including showing popups by instance, by type with dependency injection, passing parameters, and receiving results.

## IPopupService

`IPopupService` is the central service for all popup navigation operations. Access it through:

```csharp
// Static access
IPopupService.Current

// Or via dependency injection
public class MyViewModel
{
    private readonly IPopupService _popupService;

    public MyViewModel(IPopupService popupService)
    {
        _popupService = popupService;
    }
}
```

---

## Showing a Popup with its Instance

The simplest way to show a popup is by creating an instance and passing it to `PushAsync`:

```csharp
// Create the popup instance
var popup = new Toast
{
    Title = "Hello World!"
};

// Show the popup
await IPopupService.Current.PushAsync(popup);
```

### With Navigation Parameters

```csharp
var popup = new MyCustomPopup();

var parameters = new Dictionary<string, object?>
{
    { "userId", 123 },
    { "userName", "John Doe" },
    { "isAdmin", true }
};

await IPopupService.Current.PushAsync(popup, parameters);
```

The popup receives these parameters in the `OnNavigatedTo` method:

```csharp
public class MyCustomPopup : PopupPage
{
    public override void OnNavigatedTo(IReadOnlyDictionary<string, object?> parameters)
    {
        base.OnNavigatedTo(parameters);

        if (parameters.TryGetValue("userId", out var userId))
        {
            Console.WriteLine($"User ID: {userId}");
        }
    }
}
```

---

## Showing a Popup with Return Type

For popups that inherit from `PopupResultPage<T>`, use the generic overload to get a typed result:

```csharp
// FormPopup returns List<string?>
var formPopup = new FormPopup
{
    Title = "Login",
    Items = new List<FormField>
    {
        new FormField { Placeholder = "Email" },
        new FormField { Placeholder = "Password", IsPassword = true }
    }
};

// PushAsync returns the result when the popup is closed
List<string?>? result = await IPopupService.Current.PushAsync(formPopup);

if (result != null)
{
    string? email = result[0];
    string? password = result[1];
    // Process login
}
```

### Custom Result Popup

```csharp
public class ColorPickerPopup : PopupResultPage<Color>
{
    private void OnColorSelected(Color color)
    {
        SetResult(color);
        IPopupService.Current.PopAsync(this);
    }
}

// Usage
var popup = new ColorPickerPopup();
Color? selectedColor = await IPopupService.Current.PushAsync(popup);

if (selectedColor != null)
{
    // Use the selected color
}
```

---

## Showing a Popup by Type (Dependency Injection)

When popups are registered with dependency injection, you can show them by type without creating instances manually:

```csharp
// Show a popup by type - the instance is resolved from DI
await IPopupService.Current.PushAsync<MyCustomPopup>();

// With parameters
await IPopupService.Current.PushAsync<MyCustomPopup>(new Dictionary<string, object?>
{
    { "itemId", 42 }
});
```

### With Return Type

```csharp
// TPopupResult is the popup type, TResult is the return type
bool confirmed = await IPopupService.Current.PushAsync<ConfirmationPopup, bool>();

// With parameters
string? selectedOption = await IPopupService.Current.PushAsync<OptionSelectorPopup, string>(
    new Dictionary<string, object?>
    {
        { "options", new[] { "A", "B", "C" } }
    });
```

For detailed information on registering popups with DI, see the [Dependency Injection](Dependency-Injection.md) page.

---

## Navigation Parameters

Navigation parameters allow you to pass data to popups when showing them.

### Passing Parameters

```csharp
var parameters = new Dictionary<string, object?>
{
    // Primitive types
    { "id", 123 },
    { "name", "Product Name" },
    { "price", 29.99 },
    { "isAvailable", true },

    // Complex types
    { "product", new Product { Id = 1, Name = "Widget" } },
    { "items", new List<string> { "A", "B", "C" } },

    // Nullable values
    { "optionalData", null }
};

await IPopupService.Current.PushAsync(popup, parameters);
```

### Receiving Parameters in PopupPage

Override `OnNavigatedTo` in your popup:

```csharp
public class ProductDetailPopup : PopupPage
{
    private Label _nameLabel;
    private Label _priceLabel;

    public override void OnNavigatedTo(IReadOnlyDictionary<string, object?> parameters)
    {
        base.OnNavigatedTo(parameters);

        // Safe retrieval with TryGetValue
        if (parameters.TryGetValue("name", out var name))
        {
            _nameLabel.Text = name?.ToString();
        }

        if (parameters.TryGetValue("price", out var price) && price is double priceValue)
        {
            _priceLabel.Text = $"${priceValue:F2}";
        }

        // Direct access (may throw if key doesn't exist)
        // var id = (int)parameters["id"];
    }
}
```

### Receiving Parameters in ViewModel

If you're using MVVM with `IPopupViewModel`, parameters are also passed to the ViewModel:

```csharp
public class ProductDetailViewModel : IPopupViewModel
{
    public string ProductName { get; set; }
    public double Price { get; set; }

    public Task OnPopupNavigatedAsync(IReadOnlyDictionary<string, object?> parameters)
    {
        if (parameters.TryGetValue("name", out var name))
        {
            ProductName = name?.ToString() ?? string.Empty;
        }

        if (parameters.TryGetValue("price", out var price) && price is double priceValue)
        {
            Price = priceValue;
        }

        return Task.CompletedTask;
    }
}
```

For more information on ViewModel integration, see the [MVVM](MVVM.md) page.

---

## Methods to Work with Navigation

### PopAsync

Close a popup and remove it from the navigation stack:

```csharp
// Close the top-most popup
await IPopupService.Current.PopAsync();

// Close a specific popup
await IPopupService.Current.PopAsync(myPopup);
```

### Accessing the Navigation Stack

```csharp
// Get all active popups
IReadOnlyList<IPopupPage> stack = IPopupService.Current.NavigationStack;

// Check if any popups are open
bool hasPopups = stack.Count > 0;

// Get the top-most popup
IPopupPage? topPopup = stack.LastOrDefault();

// Find a specific popup type
var myPopup = stack.OfType<MyCustomPopup>().FirstOrDefault();
```

### Navigation Events

Monitor navigation activity:

```csharp
// When any popup is pushed
IPopupService.Current.PopupPushed += (sender, args) =>
{
    Console.WriteLine($"Pushed: {args.PopupPage.GetType().Name}");
};

// When any popup is popped
IPopupService.Current.PopupPopped += (sender, args) =>
{
    Console.WriteLine($"Popped: {args.PopupPage.GetType().Name}");
};

// When the stack changes (push or pop)
IPopupService.Current.StackChanged += (sender, args) =>
{
    Console.WriteLine($"Stack count: {args.Stack.Count}");
};

// Lifecycle events
IPopupService.Current.PopupOpening += (sender, args) => { /* Before animation */ };
IPopupService.Current.PopupOpened += (sender, args) => { /* After animation */ };
IPopupService.Current.PopupClosing += (sender, args) => { /* Before animation */ };
IPopupService.Current.PopupClosed += (sender, args) => { /* After animation */ };
```

---

## Closing Popups from Within

Popups can close themselves:

```csharp
public partial class MyPopup : PopupPage
{
    private async void OnCloseButtonClicked(object sender, EventArgs e)
    {
        await IPopupService.Current.PopAsync(this);
    }
}
```

For `PopupResultPage<T>`, set the result before closing:

```csharp
public partial class MyResultPopup : PopupResultPage<string>
{
    private async void OnConfirmClicked(object sender, EventArgs e)
    {
        SetResult("confirmed");
        await IPopupService.Current.PopAsync(this);
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        SetResult(null);
        await IPopupService.Current.PopAsync(this);
    }
}
```

---

## Android Back Button

On Android, pressing the system back button (or performing the back gesture) automatically closes the topmost popup. This behavior is **enabled by default** when you call `UseUXDiversPopups()`.

When the back button is pressed:
1. If there are popups on the navigation stack, the topmost popup is closed via `PopAsync()`.
2. If no popups are open, the back button behaves normally (system navigation).

### Disabling Android Back Button Close

To disable this behavior, pass `closePopupOnBackAndroid: false` when configuring the library:

```csharp
builder
    .UseMauiApp<App>()
    .UseUXDiversPopups(closePopupOnBackAndroid: false);
```
