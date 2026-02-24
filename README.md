
![UXDivers Popups](https://raw.githubusercontent.com/UXDivers/uxd-popups/refs/heads/main/img/banner.png)

# UXDivers Popups for .NET MAUI


[![NuGet](https://img.shields.io/nuget/v/UXDivers.Popups.Maui?color=3479FF)](https://www.nuget.org/packages/UXDivers.Popups.Maui) [![Downloads](https://img.shields.io/nuget/dt/UXDivers.Popups.Maui?color=3479FF)](https://www.nuget.org/packages/UXDivers.Popups.Maui) [![License](https://img.shields.io/github/license/UXDivers/uxd-popups?color=3479FF)](LICENSE)


## Introduction

- 🎨 **9 Ready-to-Use Popups** — Toast, Floater, ActionModal, SimpleText, SimpleAction, IconText, ListAction, OptionSheet, Form
- 🎬 **14 Animation Types** — Fade, Scale, Move, Rotate, and Storyboard Combinations
- 📱 **Cross-Platform** — iOS, Android, Windows, macOS
- 🎯 **MVVM Ready** — Full ViewModel and data binding support
- 💉 **Dependency Injection** — Seamless DI integration
- 🎭 **Themeable** — Dark theme included, fully customizable
- ⚡ **Lightweight** — Minimal footprint, maximum performance

---

## 🚀 Setup

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
            .UseUXDiversPopups();  // 👈 Add this line

        return builder.Build();
    }
}
```

> **Android Back Button:** By default, pressing the Android back button closes the topmost popup. To disable this behavior, pass `closePopupOnBackAndroid: false` to `UseUXDiversPopups()`. See [Navigation - Android Back Button](docs/Navigation.md#android-back-button) for details.

### 3. Add Theme Resources to App.xaml

Add the `DarkTheme` and `PopupStyles` resource dictionaries to your `App.xaml`:

```xml
<Application xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:uxd="clr-namespace:UXDivers.Popups.Maui.Controls;assembly=UXDivers.Popups.Maui"
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

## 📲 Showing a Popup

Create an instance of a popup and show it using the `IPopupService`:

```csharp
using UXDivers.Popups.Maui.Controls;

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

## 🎨 Custom Popup

Create your own popup by extending `PopupPage`:

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
    CloseWhenBackgroundIsClicked="True"
    >
    <Border 
        VerticalOptions="Center"
        HorizontalOptions="Center"
        BackgroundColor="{DynamicResource PopupBorderColor}"
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
                TextColor="{DynamicResource TextColor}"
                />

            <Label 
                Text="This is your custom popup!"
                HorizontalOptions="Center"
                TextColor="{DynamicResource TextColor}" />
        </VerticalStackLayout>
    </Border>
</uxd:PopupPage>
```

```csharp
using UXDivers.Popups.Maui;

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

---

## 📚 Learn the Advanced

For detailed documentation on advanced features, explore these resources:

### Documentation

| Topic | Description |
|-------|-------------|
| **[Popup Class](docs/Popup-Class.md)** | Core popup classes, lifecycle, and customization |
| **[Custom Popups](docs/Custom-Popups.md)** | Create your own popups with custom styling |
| **[Popup Controls](docs/Popup-Controls.md)** | All 9 pre-built popup controls explained |
| **[Navigation](docs/Navigation.md)** | How to show, close, and pass data to popups |
| **[Dependency Injection](docs/Dependency-Injection.md)** | Register popups and ViewModels with DI |
| **[MVVM](docs/MVVM.md)** | ViewModel integration and patterns |
| **[Animations](docs/Animations.md)** | Animation system and customization |
| **[API Reference](docs/API-Reference.md)** | Complete reference for all public types and methods |

### Popup Controls Reference

| Control | Docs Link |
|---------|-----------|
| Toast | **[Toast](docs/Popup-Controls.md#toast)** |
| FloaterPopup | **[FloaterPopup](docs/Popup-Controls.md#floaterpopup)** |
| SimpleTextPopup | **[SimpleTextPopup](docs/Popup-Controls.md#simpletextpopup)** |
| SimpleActionPopup | **[SimpleActionPopup](docs/Popup-Controls.md#simpleactionpopup)** |
| IconTextPopup | **[IconTextPopup](docs/Popup-Controls.md#icontextpopup)** |
| ActionModalPopup | **[ActionModalPopup](docs/Popup-Controls.md#actionmodalpopup)** |
| ListActionPopup | **[ListActionPopup](docs/Popup-Controls.md#listactionpopup)** |
| OptionSheetPopup | **[OptionSheetPopup](docs/Popup-Controls.md#optionsheetpopup)** |
| FormPopup | **[FormPopup](docs/Popup-Controls.md#formpopup)** |

### Additional Resources
- **[DemoApp](src/UXDivers.Popups.Maui.DemoApp)** - Working examples of all popup types, custom styles, and animations

---

## License

This project is licensed under the Apache License 2.0 - see the [LICENSE](LICENSE) file for details.

---

Made with ❤️ by <a href="https://uxdivers.com">UXDivers</a>
