# API Reference

Complete reference for all public types, interfaces, classes, and methods in UXDivers Popups.

---

## Index

### Core Library (UXDivers.Popups)

- [Interfaces](#interfaces)
  - [IPopupService](#ipopupservice)
  - [IPopupPage](#ipopuppage)
  - [IPopupResultPage\<T\>](#ipopupresultpaget)
  - [IPopupViewModel](#ipopupviewmodel)
  - [IPopupRegistryService](#ipopupregistryservice)
  - [IBaseAnimation](#ibaseanimation)
  - [INativePopupManager](#inativepopupmanager)
  - [IUIThreadDispatcher](#iuithreaddispatcher)
- [Classes](#classes)
  - [PopupPage](#popuppage)
  - [PopupResultPage\<T\>](#popupresultpaget)
  - [PopupEventArgs](#popupeventargs)
  - [PopupStackChangedEventArgs](#popupstackchangedeventargs)
- [Enums](#enums)
  - [EasingType](#easingtype)
  - [SafeAreaAsPadding](#safeareaaspadding)
  - [RegistryLifetime](#registrylifetime)

### MAUI Implementation (UXDivers.Popups.Maui)

- [Animations](#animations)
  - [PopupBaseAnimation](#popupbaseanimation)
  - [StoryboardAnimation](#storyboardanimation)
  - [FadeInPopupAnimation](#fadeinpopupanimation)
  - [FadeOutPopupAnimation](#fadeoutpopupanimation)
  - [FadeToPopupAnimation](#fadetopopupanimation)
  - [MoveInPopupAnimation](#moveinpopupanimation)
  - [MoveOutPopupAnimation](#moveoutpopupanimation)
  - [TranslateToPopupAnimation](#translatetopopupanimation)
  - [ScaleInPopupAnimation](#scaleinpopupanimation)
  - [ScaleOutPopupAnimation](#scaleoutpopupanimation)
  - [ScaleToPopupAnimation](#scaletopopupanimation)
  - [RotateToAnimation](#rotatetoanimation)
  - [AppearingPopupAnimation](#appearingpopupanimation)
  - [DisappearingPopupAnimation](#disappearingpopupanimation)
- [Enums (MAUI)](#enums-maui)
  - [MoveDirection](#movedirection)
  - [StoryboardStrategy](#storyboardstrategy)
- [Extension Methods](#extension-methods)
  - [HostBuilderExtensions](#hostbuilderextensions)
  - [ServiceCollectionExtensions](#servicecollectionextensions)
- [Helpers](#helpers)
  - [TypeTemplateSelector](#typetemplateselector)
  - [TypeTemplateSelectorItem](#typetemplateselectoritem)

### UI Controls (UXDivers.Popups.Maui.Controls)

- [Popup Controls](#popup-controls)
  - [Toast](#toast)
  - [FloaterPopup](#floaterpopup)
  - [SimpleTextPopup](#simpletextpopup)
  - [SimpleActionPopup](#simpleactionpopup)
  - [IconTextPopup](#icontextpopup)
  - [ActionModalPopup](#actionmodalpopup)
  - [ListActionPopup](#listactionpopup)
  - [OptionSheetPopup](#optionsheetpopup)
  - [FormPopup](#formpopup)
- [Models](#models)
  - [FormField](#formfield)
  - [OptionSheetItem](#optionsheetitem)
  - [OptionSheetGroup](#optionsheetgroup)
  - [IOptionSheetGroupableItem](#ioptionsheetgroupableitem)
- [Themes](#themes)
  - [DarkTheme](#darktheme)
  - [PopupStyles](#popupstyles)

---

# Core Library (UXDivers.Popups)

## Interfaces

### IPopupService

Central service for popup navigation operations.

**Namespace:** `UXDivers.Popups.Services`

#### Static Properties

| Property | Type | Description |
|----------|------|-------------|
| `Current` | `IPopupService` | Static instance of the popup service |

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `NavigationStack` | `IReadOnlyList<IPopupPage>` | Current popup navigation stack |

#### Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `PushAsync(IPopupPage, Dictionary<string, object?>?)` | `Task` | Push a popup instance onto the stack |
| `PushAsync<T>(IPopupResultPage<T>, Dictionary<string, object?>?)` | `Task<T?>` | Push a popup and return its result |
| `PushAsync<TPopup>(Dictionary<string, object?>?)` | `Task` | Push a popup by type (DI resolution) |
| `PushAsync<TPopup, TResult>(Dictionary<string, object?>?)` | `Task<TResult?>` | Push a popup by type and return result |
| `PopAsync(IPopupPage?)` | `Task` | Pop a popup from the stack |
| `Initialize(INativePopupManager, IUIThreadDispatcher, IViewModelAssignmentStrategy?)` | `void` | Initialize the service with platform dependencies |

#### Events

| Event | Type | Description |
|-------|------|-------------|
| `PopupPushed` | `EventHandler<PopupEventArgs>` | Raised when a popup is pushed |
| `PopupPopped` | `EventHandler<PopupEventArgs>` | Raised when a popup is popped |
| `StackChanged` | `EventHandler<PopupStackChangedEventArgs>` | Raised when stack changes |
| `PopupOpening` | `EventHandler<PopupEventArgs>` | Raised before popup opens |
| `PopupOpened` | `EventHandler<PopupEventArgs>` | Raised after popup opens |
| `PopupClosing` | `EventHandler<PopupEventArgs>` | Raised before popup closes |
| `PopupClosed` | `EventHandler<PopupEventArgs>` | Raised after popup closes |

---

### IPopupPage

Contract for popup pages.

**Namespace:** `UXDivers.Popups.Controls`

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `BackgroundOpacity` | `double?` | Overlay opacity |
| `BackgroundColor` | `object` | Background color |
| `AppearingAnimation` | `IBaseAnimation` | Animation when appearing |
| `DisappearingAnimation` | `IBaseAnimation` | Animation when disappearing |
| `CloseWhenBackgroundIsClicked` | `bool` | Auto-close on background tap |
| `BackgroundClickedCommand` | `ICommand` | Command for background tap |
| `BackgroundClickedCommandParameter` | `object` | Parameter for background command |
| `BackgroundInputTransparent` | `bool` | Allow taps through background |
| `DisableWhenIsAnimating` | `bool` | Disable interaction during animation |
| `SafeAreaAsPadding` | `SafeAreaAsPadding` | Safe area edge handling |
| `PopupContent` | `View?` | Content of the popup |
| `ActualContent` | `View?` | Actual visual content |

#### Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `OnAppearing()` | `void` | Called when popup appears |
| `OnDisappearing()` | `void` | Called when popup disappears |
| `OnNavigatedTo(IReadOnlyDictionary<string, object?>)` | `void` | Called with navigation parameters |
| `SetInteractionEnabled(bool)` | `void` | Enable/disable interactions |
| `OnPopupOpeningAsync(PopupEventArgs)` | `Task` | Called before popup opens |
| `OnPopupOpenedAsync(PopupEventArgs)` | `Task` | Called after popup opens |
| `OnPopupClosingAsync(PopupEventArgs)` | `Task` | Called before popup closes |
| `OnPopupClosedAsync(PopupEventArgs)` | `Task` | Called after popup closes |

#### Events

| Event | Type | Description |
|-------|------|-------------|
| `PopupOpening` | `EventHandler<PopupEventArgs>` | Popup is opening |
| `PopupOpened` | `EventHandler<PopupEventArgs>` | Popup has opened |
| `PopupClosing` | `EventHandler<PopupEventArgs>` | Popup is closing |
| `PopupClosed` | `EventHandler<PopupEventArgs>` | Popup has closed |
| `PopupBackgroundClicked` | `EventHandler<PopupEventArgs>` | Background was clicked |

---

### IPopupResultPage\<T\>

Contract for popups that return a typed result.

**Namespace:** `UXDivers.Popups.Controls`

**Extends:** `IPopupPage`

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Result` | `T?` | The result value |

#### Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `SetResult(T?)` | `void` | Sets the popup result |

---

### IPopupViewModel

Interface for ViewModels that receive navigation parameters.

**Namespace:** `UXDivers.Popups`

#### Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `OnPopupNavigatedAsync(IReadOnlyDictionary<string, object?>)` | `Task` | Called with navigation parameters |

---

### IPopupRegistryService

Service for registering popup-viewmodel associations.

**Namespace:** `UXDivers.Popups.Services`

#### Static Properties

| Property | Type | Description |
|----------|------|-------------|
| `Current` | `IPopupRegistryService` | Static instance of the service |

#### Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `AddTransient<TPopup, TViewModel>()` | `IPopupRegistryService` | Register popup/VM as transient |
| `AddSingleton<TPopup, TViewModel>()` | `IPopupRegistryService` | Register popup/VM as singleton |
| `AddSingleton<TPopup, TViewModel>(instance)` | `IPopupRegistryService` | Register with existing VM instance |
| `UseServiceProvider(Func<Type, object?>)` | `IPopupRegistryService` | Configure DI service provider |

---

### IBaseAnimation

Contract for popup animations.

**Namespace:** `UXDivers.Popups`

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Duration` | `int` | Animation duration in milliseconds |
| `Easing` | `EasingType` | Easing function |
| `AnimateOnlyContent` | `bool` | Animate only content or entire popup |

#### Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `RunAnimation(IPopupPage)` | `Task` | Execute the animation |
| `PrepareAnimation(IPopupPage)` | `void` | Prepare the animation state |

---

### INativePopupManager

Manages native popup display across platforms.

**Namespace:** `UXDivers.Popups.Services`

#### Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `ShowNativeViewAsync(IPopupPage)` | `Task<object>` | Display popup as native view |
| `CloseNativeViewAsync(object)` | `Task` | Close and remove native view |

---

### IUIThreadDispatcher

Framework-agnostic UI thread dispatcher.

**Namespace:** `UXDivers.Popups.Services`

#### Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `Dispatch(Action)` | `bool` | Schedule action on UI thread |
| `Dispatch(Func<Task>)` | `bool` | Schedule async action on UI thread |
| `DispatchAsync<T>(Func<T>)` | `Task<T>` | Execute function with result |
| `DispatchAsync(Func<Task>)` | `Task` | Execute async action |
| `DispatchAsync<T>(Func<Task<T>>)` | `Task<T>` | Execute async function with result |

---

## Classes

### PopupPage

Main popup control with lifecycle events and animations.

**Namespace:** `UXDivers.Popups`

**Extends:** `ContentView`

**Implements:** `IPopupPage`

#### Bindable Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `BackgroundOpacityProperty` | `double?` | null | Overlay opacity |
| `AppearingAnimationProperty` | `IBaseAnimation` | null | Appearing animation |
| `DisappearingAnimationProperty` | `IBaseAnimation` | null | Disappearing animation |
| `CloseWhenBackgroundIsClickedProperty` | `bool` | false | Close on background tap |
| `BackgroundClickedCommandProperty` | `ICommand` | null | Background click command |
| `BackgroundClickedCommandParameterProperty` | `object` | null | Command parameter |
| `BackgroundInputTransparentProperty` | `bool` | false | Input transparency |
| `DisableWhenIsAnimatingProperty` | `bool` | true | Disable during animation |
| `SafeAreaAsPaddingProperty` | `SafeAreaAsPadding` | Top\|Right\|Left\|Bottom | Safe area handling |

#### Virtual Methods

| Method | Description |
|--------|-------------|
| `OnAppearing()` | Override for appearing logic |
| `OnDisappearing()` | Override for disappearing logic |
| `OnNavigatedTo(IReadOnlyDictionary<string, object?>)` | Override for navigation parameters |
| `OnPopupOpeningAsync(PopupEventArgs)` | Override for opening logic |
| `OnPopupOpenedAsync(PopupEventArgs)` | Override for opened logic |
| `OnPopupClosingAsync(PopupEventArgs)` | Override for closing logic |
| `OnPopupClosedAsync(PopupEventArgs)` | Override for closed logic |

---

### PopupResultPage\<T\>

Generic popup that returns a typed result.

**Namespace:** `UXDivers.Popups`

**Extends:** `PopupPage`

**Implements:** `IPopupResultPage<T>`

#### Bindable Properties

| Property | Type | Description |
|----------|------|-------------|
| `ResultProperty` | `T?` | The result value |

#### Methods

| Method | Description |
|--------|-------------|
| `SetResult(T?)` | Sets the popup result before closing |

---

### PopupEventArgs

Event arguments for popup events.

**Namespace:** `UXDivers.Popups.Controls`

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `PopupPage` | `IPopupPage` | The popup associated with the event |

---

### PopupStackChangedEventArgs

Event arguments for stack changes.

**Namespace:** `UXDivers.Popups.Services`

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Stack` | `IReadOnlyList<IPopupPage>` | Current popup stack |

---

## Enums

### EasingType

Animation easing functions.

**Namespace:** `UXDivers.Popups`

| Value | Description |
|-------|-------------|
| `Default` | Default easing |
| `Linear` | Constant speed |
| `BounceIn` | Bounce effect at start |
| `BounceOut` | Bounce effect at end |
| `CubicIn` | Cubic acceleration |
| `CubicOut` | Cubic deceleration |
| `CubicInOut` | Cubic acceleration/deceleration |
| `SinIn` | Sinusoidal acceleration |
| `SinOut` | Sinusoidal deceleration |
| `SinInOut` | Sinusoidal acceleration/deceleration |
| `SpringIn` | Spring effect at start |
| `SpringOut` | Spring effect at end |

---

### SafeAreaAsPadding

Specifies which edges respect safe area insets.

**Namespace:** `UXDivers.Popups`

**Flags Enum**

| Value | Binary | Description |
|-------|--------|-------------|
| `None` | 0 | No safe area insets |
| `Top` | 1 | Top edge |
| `Left` | 2 | Left edge |
| `Right` | 4 | Right edge |
| `Bottom` | 8 | Bottom edge |
| `All` | 15 | All edges |

---

### RegistryLifetime

Service/popup registration lifetime.

**Namespace:** `UXDivers.Popups`

| Value | Description |
|-------|-------------|
| `Singleton` | Single instance reused |
| `Transient` | New instance each time |

---

# MAUI Implementation (UXDivers.Popups.Maui)

## Animations

### PopupBaseAnimation

Abstract base class for all popup animations.

**Namespace:** `UXDivers.Popups`

**Implements:** `IBaseAnimation`, `IMarkupExtension<IBaseAnimation>`

#### Bindable Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `DurationProperty` | `int` | 500 | Duration in milliseconds |
| `EasingProperty` | `EasingType` | Default | Easing function |
| `AnimateOnlyContentProperty` | `bool` | true | Animate only content |

#### Abstract Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `CreateAnimation(VisualElement, PopupPage)` | `Animation` | Creates the animation logic |
| `PrepareAnimation(VisualElement, PopupPage)` | `void` | Prepares animation state |

---

### StoryboardAnimation

Combines multiple animations with parallel or sequential execution.

**Namespace:** `UXDivers.Popups`

**Implements:** `IBaseAnimation`, `IMarkupExtension<IBaseAnimation>`

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Animation1` - `Animation5` | `IBaseAnimation?` | Animations in storyboard |
| `Strategy` | `StoryboardStrategy` | Execution strategy |
| `AnimateOnlyContent` | `bool` | Animate only content |
| `Duration` | `int` | Total duration (calculated) |
| `Easing` | `EasingType` | Easing type |

---

### FadeInPopupAnimation

Fades popup from transparent to opaque (0 → 1).

**Namespace:** `UXDivers.Popups`

**Extends:** `FadeToPopupAnimation`

---

### FadeOutPopupAnimation

Fades popup from opaque to transparent (1 → 0).

**Namespace:** `UXDivers.Popups`

**Extends:** `FadeToPopupAnimation`

---

### FadeToPopupAnimation

Base class for fade animations.

**Namespace:** `UXDivers.Popups`

**Extends:** `PopupBaseAnimation`

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `InitialOpacity` | `double?` | Starting opacity (0-1) |
| `FinalOpacity` | `double?` | Target opacity (0-1) |

---

### MoveInPopupAnimation

Moves popup in from a specified direction.

**Namespace:** `UXDivers.Popups`

**Extends:** `TranslateToPopupAnimation`

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `MoveDirection` | `MoveDirection` | Direction to move from |
| `TranslationFromCenter` | `double?` | Distance from center |

---

### MoveOutPopupAnimation

Moves popup out to a specified direction.

**Namespace:** `UXDivers.Popups`

**Extends:** `TranslateToPopupAnimation`

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `MoveDirection` | `MoveDirection` | Direction to move to |
| `TranslationFromCenter` | `double?` | Distance from center |

---

### TranslateToPopupAnimation

Base class for translation animations.

**Namespace:** `UXDivers.Popups`

**Extends:** `PopupBaseAnimation`

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `TranslationX` | `double?` | Target horizontal offset |
| `TranslationY` | `double?` | Target vertical offset |

---

### ScaleInPopupAnimation

Scales popup from smaller to normal size.

**Namespace:** `UXDivers.Popups`

**Extends:** `ScaleToPopupAnimation`

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ScaleFrom` | `double?` | 0.6 | Starting scale value |

---

### ScaleOutPopupAnimation

Scales popup from normal to smaller size.

**Namespace:** `UXDivers.Popups`

**Extends:** `ScaleToPopupAnimation`

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ScaleTo` | `double?` | 0.6 | Ending scale value |

---

### ScaleToPopupAnimation

Base class for scale animations.

**Namespace:** `UXDivers.Popups`

**Extends:** `PopupBaseAnimation`

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Scale` | `double?` | Uniform scale for X and Y |
| `ScaleX` | `double?` | Horizontal scale |
| `ScaleY` | `double?` | Vertical scale |

---

### RotateToAnimation

Animates rotation on Z, X, and/or Y axes.

**Namespace:** `UXDivers.Popups`

**Extends:** `PopupBaseAnimation`

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Rotation` | `double?` | Z-axis rotation (degrees) |
| `RotationX` | `double?` | X-axis rotation (degrees) |
| `RotationY` | `double?` | Y-axis rotation (degrees) |

---

### AppearingPopupAnimation

Combined animation: fade in + scale in + move in (parallel).

**Namespace:** `UXDivers.Popups`

**Extends:** `StoryboardAnimation`

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `AppearingDirection` | `MoveDirection` | Direction popup appears from |

---

### DisappearingPopupAnimation

Combined animation: fade out + scale out + move out (parallel).

**Namespace:** `UXDivers.Popups`

**Extends:** `StoryboardAnimation`

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `DisappearingDirection` | `MoveDirection` | Direction popup disappears to |

---

## Enums (MAUI)

### MoveDirection

Direction for move animations.

**Namespace:** `UXDivers.Popups`

| Value | Description |
|-------|-------------|
| `Left` | From/to left side |
| `Top` | From/to top side |
| `Right` | From/to right side |
| `Bottom` | From/to bottom side |

---

### StoryboardStrategy

Animation storyboard execution strategy.

**Namespace:** `UXDivers.Popups`

| Value | Description |
|-------|-------------|
| `RunAllAtStart` | Run all animations in parallel |
| `RunAllSequentially` | Run animations one after another |

---

## Extension Methods

### HostBuilderExtensions

Extension methods for MAUI app configuration.

**Namespace:** `UXDivers.Popups`

| Method | Returns | Description |
|--------|---------|-------------|
| `UseUXDPopupsForMaui(this MauiAppBuilder)` | `MauiAppBuilder` | Configure core UXD Popups |
| `UXDPopupsUseMauiServices(this IServiceCollection)` | `IServiceCollection` | Configure registry with MAUI services |

**Namespace:** `UXDivers.Popups.Maui.Controls`

| Method | Returns | Description |
|--------|---------|-------------|
| `UseUXDiversPopups(this MauiAppBuilder, bool closePopupOnBackAndroid = true)` | `MauiAppBuilder` | Configure UXD Popups. On Android, closes the topmost popup on back button press by default. |

---

### ServiceCollectionExtensions

DI registration extension methods.

**Namespace:** `UXDivers.Popups`

| Method | Returns | Description |
|--------|---------|-------------|
| `AddTransientPopup<TPopup>()` | `IServiceCollection` | Register popup as transient |
| `AddTransientPopup<TPopup, TViewModel>()` | `IServiceCollection` | Register popup/VM as transient |
| `AddSingletonPopup<TPopup>()` | `IServiceCollection` | Register popup as singleton |
| `AddSingletonPopup<TPopup, TViewModel>()` | `IServiceCollection` | Register popup/VM as singleton |
| `AddSingletonPopup<TPopup>(instance)` | `IServiceCollection` | Register popup instance |
| `AddSingletonPopup<TPopup, TViewModel>(vmInstance)` | `IServiceCollection` | Register with VM instance |
| `AddSingletonPopup<TPopup, TViewModel>(popupInstance)` | `IServiceCollection` | Register popup instance with VM type |
| `AddSingletonPopup<TPopup, TViewModel>(popup, vm)` | `IServiceCollection` | Register both instances |

---

## Helpers

### TypeTemplateSelector

DataTemplate selector based on item type.

**Namespace:** `UXDivers.Popups`

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Items` | `List<TypeTemplateSelectorItem>` | Type-to-template mappings |

---

### TypeTemplateSelectorItem

Type-to-template mapping item.

**Namespace:** `UXDivers.Popups`

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `TargetType` | `Type?` | Type to match |
| `Template` | `DataTemplate?` | Template for the type |

---

# UI Controls (UXDivers.Popups.Maui.Controls)

## Popup Controls

### Toast

Lightweight notification popup.

**Namespace:** `UXDivers.Popups.Maui.Controls`

**Extends:** `PopupPage`

#### Bindable Properties

| Property | Type | Description |
|----------|------|-------------|
| `IconText` | `string` | Icon glyph |
| `IconColor` | `Color` | Icon color |
| `Title` | `string` | Title text |

---

### FloaterPopup

Floating notification with icon and message.

**Namespace:** `UXDivers.Popups.Maui.Controls`

**Extends:** `PopupPage`

#### Bindable Properties

| Property | Type | Description |
|----------|------|-------------|
| `IconText` | `string` | Icon glyph |
| `IconColor` | `Color` | Icon color |
| `Title` | `string` | Title text |
| `Text` | `string` | Main text content |

---

### SimpleTextPopup

Simple informational popup with title and text.

**Namespace:** `UXDivers.Popups.Maui.Controls`

**Extends:** `PopupPage`

#### Bindable Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `CloseButtonIconText` | `string` | null | Close button icon |
| `CloseButtonIconColor` | `Color` | null | Close button color |
| `CloseButtonCommand` | `ICommand` | PopAsync | Close button command |
| `Title` | `string` | null | Title text |
| `Text` | `string` | null | Main text content |

---

### SimpleActionPopup

Two-button confirmation dialog.

**Namespace:** `UXDivers.Popups.Maui.Controls`

**Extends:** `PopupPage`

#### Bindable Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Title` | `string` | null | Title text |
| `Text` | `string` | null | Main text content |
| `ActionButtonCommand` | `ICommand` | PopAsync | Primary button command |
| `ActionButtonText` | `string` | null | Primary button text |
| `ShowActionButton` | `bool` | true | Show primary button |
| `SecondaryActionButtonCommand` | `ICommand` | PopAsync | Secondary button command |
| `SecondaryActionButtonText` | `string` | null | Secondary button text |
| `ShowSecondaryActionButton` | `bool` | true | Show secondary button |

---

### IconTextPopup

Popup with prominent icon, title, and text content.

**Namespace:** `UXDivers.Popups.Maui.Controls`

**Extends:** `PopupPage`

#### Bindable Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `IconText` | `string` | null | Icon glyph |
| `IconColor` | `Color` | null | Icon color |
| `Title` | `string` | null | Title text |
| `Text` | `string` | null | Main text content |
| `ActionButtonCommand` | `ICommand` | PopAsync | Action button command |
| `ActionButtonText` | `string` | null | Action button text |
| `ShowActionButton` | `bool` | true | Show action button |

---

### ActionModalPopup

Modal popup with title, close button, and customizable action area.

**Namespace:** `UXDivers.Popups.Maui.Controls`

**Extends:** `PopupPage`

#### Bindable Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Title` | `string` | null | Title text |
| `CloseButtonIconText` | `string` | null | Close button icon |
| `CloseButtonIconColor` | `Color` | null | Close button color |
| `CloseButtonCommand` | `ICommand` | PopAsync | Close button command |
| `ActionButtonCommand` | `ICommand` | PopAsync | Action button command |
| `ActionButtonText` | `string` | null | Action button text |
| `ShowActionButton` | `bool` | true | Show action button |

---

### ListActionPopup

Popup displaying a scrollable list with action button.

**Namespace:** `UXDivers.Popups.Maui.Controls`

**Extends:** `PopupPage`

#### Bindable Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Title` | `string` | null | Title text |
| `CloseButtonIconText` | `string` | null | Close button icon |
| `CloseButtonIconColor` | `Color` | null | Close button color |
| `CloseButtonCommand` | `ICommand` | PopAsync | Close button command |
| `ActionButtonCommand` | `ICommand` | PopAsync | Action button command |
| `ActionButtonText` | `string` | null | Action button text |
| `ShowActionButton` | `bool` | true | Show action button |
| `ItemDataTemplate` | `DataTemplate` | null | Template for list items |
| `ItemsSource` | `IEnumerable` | null | Items collection |

---

### OptionSheetPopup

Bottom sheet popup with grouped selectable options.

**Namespace:** `UXDivers.Popups.Maui.Controls`

**Extends:** `PopupPage`

#### Bindable Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Title` | `string` | null | Title text |
| `CloseButtonIconText` | `string` | null | Close button icon |
| `CloseButtonIconColor` | `Color` | null | Close button color |
| `CloseButtonCommand` | `ICommand` | PopAsync | Close button command |
| `Items` | `IEnumerable` | null | Options collection |
| `ItemDataTemplate` | `DataTemplate` | null | Template for option items |

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Groups` | `IEnumerable<OptionSheetGroup>?` | Computed groups (read-only) |

---

### FormPopup

Form popup for collecting user input. Returns `List<string?>`.

**Namespace:** `UXDivers.Popups.Maui.Controls`

**Extends:** `PopupResultPage<List<string?>>`

#### Bindable Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Title` | `string` | null | Title text |
| `Text` | `string` | null | Descriptive text |
| `ActionButtonCommand` | `ICommand` | OnMainActionClicked | Submit button command |
| `ActionButtonText` | `string` | null | Submit button text |
| `ShowActionButton` | `bool` | true | Show submit button |
| `SecondaryActionText` | `string` | null | Secondary action label |
| `SecondaryActionLinkText` | `string` | null | Secondary action link text |
| `SecondaryActionLinkCommand` | `ICommand` | null | Secondary action command |
| `Items` | `IEnumerable` | null | Form fields collection |
| `ItemDataTemplate` | `DataTemplate` | null | Template for form fields |

---

## Models

### FormField

Model for form input fields.

**Namespace:** `UXDivers.Popups.Maui.Controls`

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Placeholder` | `string?` | Placeholder text |
| `Icon` | `string?` | Field icon |
| `IconColor` | `Color?` | Icon color |
| `Value` | `string?` | Current field value |
| `IsPassword` | `bool` | Mask input as password |

---

### OptionSheetItem

Selectable option item.

**Namespace:** `UXDivers.Popups.Maui.Controls`

**Implements:** `IOptionSheetGroupableItem`

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Text` | `string?` | Display text |
| `Icon` | `string?` | Item icon |
| `IconColor` | `Color?` | Icon color |
| `Command` | `ICommand?` | Selection command |
| `CommandParameter` | `object?` | Command parameter |
| `GroupName` | `string?` | Group for categorization |

---

### OptionSheetGroup

Container for grouped options.

**Namespace:** `UXDivers.Popups.Maui.Controls`

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Items` | `IEnumerable<object>` | Items in this group |

---

### IOptionSheetGroupableItem

Contract for groupable option items.

**Namespace:** `UXDivers.Popups.Maui.Controls`

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `GroupName` | `string?` | Group name for categorization |

---

## Themes

### DarkTheme

ResourceDictionary with dark theme colors, spacing, and fonts.

**Namespace:** `UXDivers.Popups.Maui.Controls`

#### Color Resources

| Key | Default | Description |
|-----|---------|-------------|
| `PrimaryColor` | #3479FF | Primary accent color |
| `PrimaryVariantColor` | #172949 | Primary variant |
| `BackgroundColor` | #0A0A0A | Main background |
| `BackgroundSecondaryColor` | #181A1F | Secondary background |
| `BackgroundTertiaryColor` | #262930 | Tertiary background |
| `PopupBackdropColor` | #AA000000 | Popup overlay |
| `PopupBorderColor` | #262930 | Popup border |
| `TextColor` | #FFFFFF | Primary text |
| `TextSecondaryColor` | #AAB3CD | Secondary text |
| `TextTertiaryColor` | #8B95AE | Tertiary text |
| `EntryPlaceholderColor` | #5D5D5D | Entry placeholder |

#### Font Resources

| Key | Description |
|-----|-------------|
| `IconsFontFamily` | Icons font family name |
| `AppFontFamily` | Main font family name |
| `AppSemiBoldFamily` | Semi-bold font family name |
| `UXDPopupsCloseIconButton` | Close icon glyph |
| `UXDPopupsCheckCircleIconButton` | Check circle icon glyph |

#### Spacing Resources

| Key | Default | Description |
|-----|---------|-------------|
| `AirSpacing` | 30 | General air spacing |
| `PopupAirSpacing` | 24 | Popup padding |
| `SpacingXBig` | 24 | Extra large spacing |
| `SpacingBig` | 20 | Large spacing |
| `SpacingMedium` | 16 | Medium spacing |
| `SpacingSmall` | 12 | Small spacing |

#### Corner Radius Resources

| Key | Default | Description |
|-----|---------|-------------|
| `CornerRadiusXBig` | 20 | Extra large radius |
| `CornerRadiusBig` | 16 | Large radius |
| `BaseButtonCornerRadius` | 20 | Button corner radius |

---

### PopupStyles

ResourceDictionary with control templates and styles for all popup types.

**Namespace:** `UXDivers.Popups.Maui.Controls`

#### Available Styles

| Key | Target Type | Description |
|-----|-------------|-------------|
| `PopupBaseStyle` | `PopupPage` | Base style for all popups |
| `DefaultToastStyle` | `Toast` | Default toast style |
| `DefaultFloaterPopupStyle` | `FloaterPopup` | Default floater style |
| `DefaultSimpleTextPopupStyle` | `SimpleTextPopup` | Default simple text style |
| `DefaultSimpleActionPopupStyle` | `SimpleActionPopup` | Default simple action style |
| `DefaultIconTextPopupStyle` | `IconTextPopup` | Default icon text style |
| `DefaultActionModalPopupStyle` | `ActionModalPopup` | Default action modal style |
| `DefaultListActionPopupStyle` | `ListActionPopup` | Default list action style |
| `DefaultOptionSheetPopupStyle` | `OptionSheetPopup` | Default option sheet style |
| `DefaultFormPopupStyle` | `FormPopup` | Default form style |

#### Helper Styles

| Key | Target Type | Description |
|-----|-------------|-------------|
| `LabelDefaultStyle` | `Label` | Default label style |
| `TitleStyle` | `Label` | Title text style |
| `MainTitleStyle` | `Label` | Main title style |
| `MainSubTitleStyle` | `Label` | Main subtitle style |
| `SubTitleStyle` | `Label` | Subtitle style |
| `CloseButtonStyle` | `Label` | Close button style |
| `PrimaryActionButtonStyle` | `Button` | Primary button style |
| `SecondaryActionButtonStyle` | `Button` | Secondary button style |
| `CardStyle` | `Border` | Card container style |
| `EntryDefaultStyle` | `Entry` | Default entry style |
