# UXDivers Popups for .NET MAUI

Welcome to the official documentation for **UXDivers Popups** - a comprehensive popup library for .NET MAUI applications featuring rich animations, customizable controls, and full MVVM support.

## Introduction

UXDivers Popups provides a complete popup management solution for .NET MAUI applications. The library offers:

- **9 Ready-to-Use Popup Controls** - Toast, Floater, ActionModal, SimpleText, SimpleAction, IconText, ListAction, OptionSheet, and Form
- **14 Animation Types** - Fade, Scale, Move, Rotate, and Storyboard combinations
- **Cross-Platform Support** - iOS, Android, Windows, and macOS
- **MVVM Ready** - Full ViewModel and data binding support with automatic ViewModel creation
- **Dependency Injection** - Seamless integration with .NET MAUI's DI container
- **Themeable** - Dark theme included, fully customizable through styles and templates

### Documentation Pages

| Page | Description |
|------|-------------|
| [Popup Class](Popup-Class.md) | Core popup classes, lifecycle, and customization |
| [Custom Popups](Custom-Popups.md) | Create your own popups with custom styling |
| [Popup Controls](Popup-Controls.md) | All 9 pre-built popup controls explained |
| [Navigation](Navigation.md) | How to show, close, and pass data to popups |
| [Dependency Injection](Dependency-Injection.md) | Register popups and ViewModels with DI |
| [MVVM](MVVM.md) | ViewModel integration and patterns |
| [Animations](Animations.md) | Animation system and customization |
| [API Reference](API-Reference.md) | Complete reference for all public types and methods |

---

## Setup

Follow these steps to start using the library in your project:

### 1. Install the NuGet Package

```bash
dotnet add package UXDivers.Popups.Maui
```

### 2. Configure MauiProgram.cs

Call the `UseUXDiversPopups` extension method in your `MauiProgram.cs`:

```csharp
using UXDivers.Popups.Maui.Controls;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseUXDiversPopups();  // Add this line

        return builder.Build();
    }
}
```

> **Android Back Button:** By default, pressing the Android back button closes the topmost popup. To disable this behavior, pass `closePopupOnBackAndroid: false` to `UseUXDiversPopups()`. See [Navigation - Android Back Button](Navigation.md#android-back-button) for details.

### 3. Add Theme Resources to App.xaml

Add the `DarkTheme` and `PopupStyles` resource dictionaries to your `App.xaml`:

```xml
<Application xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:uxd="clr-namespace:UXDivers.Popups.Maui.Controls;assembly=UXDivers.Popups.Maui.Controls"
             x:Class="YourApp.App">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <uxd:DarkTheme />
                <uxd:PopupStyles />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

### 4. Optional: Customize Resources

Override these resource keys in your `App.xaml` to customize the popups:

| Resource Key | Description |
|--------------|-------------|
| `IconsFontFamily` | The name of the icons font family |
| `AppFontFamily` | The name of the main font family |
| `AppSemiBoldFamily` | The name of the semi-bold font family |
| `UXDPopupsCloseIconButton` | The glyph for close icons in popups |
| `UXDPopupsCheckCircleIconButton` | The glyph for circled check icons in popups |

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <uxd:DarkTheme />
            <uxd:PopupStyles />
        </ResourceDictionary.MergedDictionaries>

        <!-- Font Customization -->
        <x:String x:Key="IconsFontFamily">MaterialIcons</x:String>
        <x:String x:Key="AppFontFamily">OpenSans-Regular</x:String>
        <x:String x:Key="AppSemiBoldFamily">OpenSans-SemiBold</x:String>

        <!-- Icon Glyphs -->
        <x:String x:Key="UXDPopupsCloseIconButton">&#xE5CD;</x:String>
        <x:String x:Key="UXDPopupsCheckCircleIconButton">&#xE86C;</x:String>
    </ResourceDictionary>
</Application.Resources>
```

---

## Getting Started

Create an instance of a popup and show it using `IPopupService`:

```csharp
using UXDivers.Popups.Maui.Controls;
using UXDivers.Popups.Services;

public async void OnShowPopupClicked()
{
    var popup = new Toast()
    {
        Title = "Update Success"
    };

    await IPopupService.Current.PushAsync(popup);
}
```

### Available Popup Types

| Popup | Description |
|-------|-------------|
| `Toast` | Brief notification with icon and title |
| `SimpleTextPopup` | Informational popup with title and text |
| `SimpleActionPopup` | Confirmation dialog with two buttons |
| `IconTextPopup` | Prominent icon with title, text, and action |
| `FloaterPopup` | Floating alert with icon and message |
| `ActionModalPopup` | Modal with close button and action area |
| `ListActionPopup` | Scrollable list with action button |
| `OptionSheetPopup` | Bottom sheet with selectable options |
| `FormPopup` | User input form returning results |

---

## Create a Custom Popup

Create your own popup by extending `PopupPage`:

**MyCustomPopup.xaml**
```xml
<?xml version="1.0" encoding="utf-8" ?>
<uxd:PopupPage
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:uxd="clr-namespace:UXDivers.Popups.Maui;assembly=UXDivers.Popups.Maui"
    x:Class="YourNamespace.MyCustomPopup"
    BackgroundColor="{DynamicResource PopupBackdropColor}"
    AppearingAnimation="{uxd:FadeInPopupAnimation Duration=300}"
    DisappearingAnimation="{uxd:FadeOutPopupAnimation Duration=300}"
    CloseWhenBackgroundIsClicked="True">

    <Border
        VerticalOptions="Center"
        HorizontalOptions="Center"
        BackgroundColor="{DynamicResource BackgroundSecondaryColor}"
        Stroke="{DynamicResource PopupBorderColor}"
        StrokeThickness="1">
        <Border.StrokeShape>
            <RoundRectangle CornerRadius="16" />
        </Border.StrokeShape>

        <VerticalStackLayout Padding="24" Spacing="16">
            <Label
                Text="Welcome!"
                FontSize="24"
                HorizontalOptions="Center"
                TextColor="{DynamicResource TextColor}" />

            <Label
                Text="This is your custom popup!"
                HorizontalOptions="Center"
                TextColor="{DynamicResource TextSecondaryColor}" />

            <Button
                Text="Close"
                Clicked="OnCloseClicked" />
        </VerticalStackLayout>
    </Border>
</uxd:PopupPage>
```

**MyCustomPopup.xaml.cs**
```csharp
using UXDivers.Popups;
using UXDivers.Popups.Services;

namespace YourApp.Popups;

public partial class MyCustomPopup : PopupPage
{
    public MyCustomPopup()
    {
        InitializeComponent();
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await IPopupService.Current.PopAsync(this);
    }
}
```

For more advanced customization options including inheriting default styles, overriding control templates, and theme customization, see the [Custom Popups](Custom-Popups.md) page.

---

## Navigation

The library uses `IPopupService` for all popup navigation operations. Access it through `IPopupService.Current` or inject it via dependency injection.

### Basic Navigation

```csharp
// Show a popup
await IPopupService.Current.PushAsync(myPopup);

// Close the current popup
await IPopupService.Current.PopAsync();

// Close a specific popup
await IPopupService.Current.PopAsync(myPopup);
```

### Navigation with Parameters

```csharp
var parameters = new Dictionary<string, object?>
{
    { "userId", 123 },
    { "userName", "John" }
};

await IPopupService.Current.PushAsync(myPopup, parameters);
```

### Getting Results from Popups

```csharp
var formPopup = new FormPopup { /* ... */ };
List<string?>? result = await IPopupService.Current.PushAsync(formPopup);

if (result != null)
{
    // Process form results
}
```

For comprehensive navigation documentation including DI-based navigation and parameter handling, see the [Navigation](Navigation.md) page.
