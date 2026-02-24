# Popup Controls

This page documents all 9 pre-built popup controls included in the UXDivers Popups library.

---

## Toast

A lightweight notification popup with an icon and title, typically shown briefly at the top of the screen.

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `IconText` | `string` | Icon glyph displayed in the toast |
| `IconColor` | `Color` | Color of the icon |
| `Title` | `string` | Title text displayed in the toast |

### Default Animation

- **Appearing**: Fade in (400ms)
- **Disappearing**: Fade out (300ms)

### Usage

```csharp
var toast = new Toast
{
    Title = "Changes saved successfully!",
    IconColor = Colors.Green
};

// Show toast without waiting for it to close
await IPopupService.Current.PushAsync(toast, waitUntilClosed: false);

// Auto-dismiss after delay
await Task.Delay(2000);
await IPopupService.Current.PopAsync(toast);
```

### XAML

```xml
<uxd:Toast
    Title="Item added to cart"
    IconColor="{DynamicResource PrimaryColor}" />
```

---

## FloaterPopup

A floating popup with an icon, title, and text content, typically displayed at the top of the screen for notifications with more detail.

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `IconText` | `string` | Icon glyph displayed in the popup |
| `IconColor` | `Color` | Color of the icon |
| `Title` | `string` | Title text |
| `Text` | `string` | Main text content |

### Default Animation

- **Appearing**: Move in from bottom (300ms, CubicOut)
- **Disappearing**: Move out to bottom (400ms, CubicIn)

### Usage

```csharp
var floater = new FloaterPopup
{
    Title = "New Message",
    Text = "You have received a new message from John.",
    IconColor = Colors.Blue
};

await IPopupService.Current.PushAsync(floater);
```

### XAML

```xml
<uxd:FloaterPopup
    Title="Download Complete"
    Text="Your file has been downloaded successfully."
    IconColor="Green" />
```

---

## SimpleTextPopup

A simple informational popup displaying a title, text content, and a close button.

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Title` | `string` | Title text |
| `Text` | `string` | Main text content |
| `CloseButtonIconText` | `string` | Icon glyph for the close button |
| `CloseButtonIconColor` | `Color` | Color of the close button icon |
| `CloseButtonCommand` | `ICommand` | Command executed when close button is clicked (default: PopAsync) |

### Default Animation

- **Appearing**: Fade in (300ms)
- **Disappearing**: Fade out (200ms)

### Usage

```csharp
var popup = new SimpleTextPopup
{
    Title = "About This App",
    Text = "This is a sample application demonstrating UXDivers Popups for .NET MAUI."
};

await IPopupService.Current.PushAsync(popup);
```

### XAML

```xml
<uxd:SimpleTextPopup
    Title="Privacy Notice"
    Text="We collect data to improve your experience. See our privacy policy for details." />
```

---

## SimpleActionPopup

A confirmation dialog with a title, text, and up to two action buttons for user decisions.

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Title` | `string` | Title text |
| `Text` | `string` | Main text content |
| `ActionButtonCommand` | `ICommand` | Primary button command (default: PopAsync) |
| `ActionButtonText` | `string` | Primary button text |
| `ShowActionButton` | `bool` | Show/hide primary button (default: true) |
| `SecondaryActionButtonCommand` | `ICommand` | Secondary button command (default: PopAsync) |
| `SecondaryActionButtonText` | `string` | Secondary button text |
| `ShowSecondaryActionButton` | `bool` | Show/hide secondary button (default: true) |

### Default Animation

- **Appearing**: Move in from right (400ms, SpringOut)
- **Disappearing**: Scale + Move out sequence

### Usage

```csharp
var popup = new SimpleActionPopup
{
    Title = "Delete Item?",
    Text = "This action cannot be undone.",
    ActionButtonText = "Delete",
    SecondaryActionButtonText = "Cancel",
    ActionButtonCommand = new Command(async () =>
    {
        // Delete logic here
        await IPopupService.Current.PopAsync();
    })
};

await IPopupService.Current.PushAsync(popup);
```

### XAML

```xml
<uxd:SimpleActionPopup
    Title="Confirm Purchase"
    Text="You are about to purchase this item for $9.99"
    ActionButtonText="Buy Now"
    SecondaryActionButtonText="Cancel" />
```

---

## IconTextPopup

A popup with a prominent icon, title, text content, and an optional action button. Useful for success/error messages.

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `IconText` | `string` | Icon glyph (displayed large) |
| `IconColor` | `Color` | Color of the icon |
| `Title` | `string` | Title text |
| `Text` | `string` | Main text content |
| `ActionButtonCommand` | `ICommand` | Action button command (default: PopAsync) |
| `ActionButtonText` | `string` | Action button text |
| `ShowActionButton` | `bool` | Show/hide action button (default: true) |

### Default Animation

- **Appearing**: Move in from right (300ms, SinIn)
- **Disappearing**: Move out to left (300ms, SinOut)

### Usage

```csharp
var popup = new IconTextPopup
{
    Title = "Payment Successful!",
    Text = "Your order has been placed and will be delivered soon.",
    IconColor = Colors.Green,
    ActionButtonText = "Continue Shopping"
};

await IPopupService.Current.PushAsync(popup);
```

### XAML

```xml
<uxd:IconTextPopup
    Title="Error Occurred"
    Text="Unable to connect to the server. Please try again later."
    IconColor="Red"
    ActionButtonText="Retry" />
```

---

## ActionModalPopup

A modal popup with a title, close button, custom content area, and an action button. Ideal for forms or detailed content.

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Title` | `string` | Title text |
| `CloseButtonIconText` | `string` | Icon glyph for close button |
| `CloseButtonIconColor` | `Color` | Color of close button icon |
| `CloseButtonCommand` | `ICommand` | Close button command (default: PopAsync) |
| `ActionButtonCommand` | `ICommand` | Action button command (default: PopAsync) |
| `ActionButtonText` | `string` | Action button text |
| `ShowActionButton` | `bool` | Show/hide action button (default: true) |

### Content

Use the `PopupContent` property or XAML content to add custom content:

```xml
<uxd:ActionModalPopup Title="Settings">
    <!-- Your custom content here -->
    <VerticalStackLayout>
        <Label Text="Volume" />
        <Slider />
    </VerticalStackLayout>
</uxd:ActionModalPopup>
```

### Default Animation

- **Appearing**: Move in from bottom (300ms, CubicOut)
- **Disappearing**: Move out to bottom (400ms, CubicIn)

### Usage

```csharp
var popup = new ActionModalPopup
{
    Title = "Edit Profile",
    ActionButtonText = "Save Changes",
    PopupContent = new VerticalStackLayout
    {
        Children =
        {
            new Entry { Placeholder = "Name" },
            new Entry { Placeholder = "Email" }
        }
    }
};

await IPopupService.Current.PushAsync(popup);
```

---

## ListActionPopup

A popup displaying a scrollable list of items with a title, close button, and action button.

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Title` | `string` | Title text |
| `CloseButtonIconText` | `string` | Icon glyph for close button |
| `CloseButtonIconColor` | `Color` | Color of close button icon |
| `CloseButtonCommand` | `ICommand` | Close button command (default: PopAsync) |
| `ActionButtonCommand` | `ICommand` | Action button command (default: PopAsync) |
| `ActionButtonText` | `string` | Action button text |
| `ShowActionButton` | `bool` | Show/hide action button (default: true) |
| `ItemsSource` | `IEnumerable` | Collection of items to display |
| `ItemDataTemplate` | `DataTemplate` | Template for rendering each item |

### Default Animation

- **Appearing**: Fade in (300ms)
- **Disappearing**: Fade out (200ms)

### Usage

```csharp
var items = new List<string> { "Option 1", "Option 2", "Option 3" };

var popup = new ListActionPopup
{
    Title = "Select an Option",
    ItemsSource = items,
    ItemDataTemplate = new DataTemplate(() =>
    {
        var label = new Label();
        label.SetBinding(Label.TextProperty, ".");
        return label;
    }),
    ActionButtonText = "Confirm"
};

await IPopupService.Current.PushAsync(popup);
```

### XAML with Custom Template

```xml
<uxd:ListActionPopup
    Title="Notifications"
    ActionButtonText="Mark All Read">
    <uxd:ListActionPopup.ItemDataTemplate>
        <DataTemplate>
            <HorizontalStackLayout Spacing="12">
                <Label Text="{Binding Icon}" FontFamily="{DynamicResource IconsFontFamily}" />
                <VerticalStackLayout>
                    <Label Text="{Binding Title}" Style="{DynamicResource TitleStyle}" />
                    <Label Text="{Binding Message}" Style="{DynamicResource SubTitleStyle}" />
                </VerticalStackLayout>
            </HorizontalStackLayout>
        </DataTemplate>
    </uxd:ListActionPopup.ItemDataTemplate>
</uxd:ListActionPopup>
```

---

## OptionSheetPopup

A bottom sheet popup displaying grouped selectable options. Items can be automatically grouped using the `GroupName` property.

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Title` | `string` | Title text |
| `CloseButtonIconText` | `string` | Icon glyph for close button |
| `CloseButtonIconColor` | `Color` | Color of close button icon |
| `CloseButtonCommand` | `ICommand` | Close button command (default: PopAsync) |
| `Items` | `IEnumerable` | Collection of option items |
| `ItemDataTemplate` | `DataTemplate` | Template for rendering each option |
| `Groups` | `IEnumerable<OptionSheetGroup>` | Computed groups from Items (read-only) |

### OptionSheetItem Model

```csharp
public class OptionSheetItem : IOptionSheetGroupableItem
{
    public string? Text { get; set; }
    public string? Icon { get; set; }
    public Color? IconColor { get; set; }
    public ICommand? Command { get; set; }
    public object? CommandParameter { get; set; }
    public string? GroupName { get; set; }  // For automatic grouping
}
```

### Default Animation

- **Appearing**: Move in from bottom (300ms, CubicOut)
- **Disappearing**: Move out to bottom (400ms, CubicIn)

### Usage

```csharp
var options = new List<OptionSheetItem>
{
    new OptionSheetItem
    {
        Text = "Share",
        Icon = "\uE80D",
        GroupName = "Actions",
        Command = new Command(() => { /* Share logic */ })
    },
    new OptionSheetItem
    {
        Text = "Copy Link",
        Icon = "\uE8C8",
        GroupName = "Actions",
        Command = new Command(() => { /* Copy logic */ })
    },
    new OptionSheetItem
    {
        Text = "Delete",
        Icon = "\uE872",
        IconColor = Colors.Red,
        GroupName = "Danger",
        Command = new Command(() => { /* Delete logic */ })
    }
};

var popup = new OptionSheetPopup
{
    Title = "Options",
    Items = options
};

await IPopupService.Current.PushAsync(popup);
```

---

## FormPopup

A form popup for collecting user input from multiple fields. Returns a `List<string?>` with the values.

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Title` | `string` | Title text |
| `Text` | `string` | Descriptive text below the title |
| `ActionButtonCommand` | `ICommand` | Submit button command |
| `ActionButtonText` | `string` | Submit button text |
| `ShowActionButton` | `bool` | Show/hide submit button (default: true) |
| `SecondaryActionText` | `string` | Secondary action label text |
| `SecondaryActionLinkText` | `string` | Secondary action link text |
| `SecondaryActionLinkCommand` | `ICommand` | Secondary action command |
| `Items` | `IEnumerable` | Collection of form fields |
| `ItemDataTemplate` | `DataTemplate` | Template for form fields |

### FormField Model

```csharp
public class FormField
{
    public string? Placeholder { get; set; }
    public string? Icon { get; set; }
    public Color? IconColor { get; set; }
    public string? Value { get; set; }
    public bool IsPassword { get; set; }
}
```

### Default Animation

- **Appearing**: Move in from bottom (500ms, CubicOut)
- **Disappearing**: Move out to top (500ms, CubicIn)

### Usage

```csharp
var fields = new List<FormField>
{
    new FormField
    {
        Placeholder = "Email",
        Icon = "\uE0BE"
    },
    new FormField
    {
        Placeholder = "Password",
        Icon = "\uE897",
        IsPassword = true
    }
};

var popup = new FormPopup
{
    Title = "Sign In",
    Text = "Enter your credentials",
    Items = fields,
    ActionButtonText = "Sign In",
    SecondaryActionText = "Don't have an account?",
    SecondaryActionLinkText = "Sign Up"
};

List<string?>? result = await IPopupService.Current.PushAsync(popup);

if (result != null && result.Count >= 2)
{
    string? email = result[0];
    string? password = result[1];
    // Process login
}
```

### XAML with Custom Field Template

```xml
<uxd:FormPopup
    Title="Contact Us"
    Text="We'd love to hear from you"
    ActionButtonText="Send Message">
    <uxd:FormPopup.ItemDataTemplate>
        <DataTemplate x:DataType="uxd:FormField">
            <Border BackgroundColor="{DynamicResource BackgroundTertiaryColor}" Padding="12">
                <Entry
                    Placeholder="{Binding Placeholder}"
                    Text="{Binding Value, Mode=TwoWay}"
                    IsPassword="{Binding IsPassword}" />
            </Border>
        </DataTemplate>
    </uxd:FormPopup.ItemDataTemplate>
</uxd:FormPopup>
```

---

## Default Styles

Each popup control has a default style defined in `PopupStyles.xaml`:

| Control | Default Style Key |
|---------|-------------------|
| Toast | `DefaultToastStyle` |
| FloaterPopup | `DefaultFloaterPopupStyle` |
| SimpleTextPopup | `DefaultSimpleTextPopupStyle` |
| SimpleActionPopup | `DefaultSimpleActionPopupStyle` |
| IconTextPopup | `DefaultIconTextPopupStyle` |
| ActionModalPopup | `DefaultActionModalPopupStyle` |
| ListActionPopup | `DefaultListActionPopupStyle` |
| OptionSheetPopup | `DefaultOptionSheetPopupStyle` |
| FormPopup | `DefaultFormPopupStyle` |

All default styles inherit from `PopupBaseStyle`, which sets common properties like background color, close-on-background-click behavior, and default animations.

For customization options, see the [Custom Popups](Custom-Popups.md) page.
