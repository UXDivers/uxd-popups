using UXDivers.Popups.Services;

namespace UXDivers.Popups.Maui;

/// <summary>
/// Extension methods for configuring UXD Popups in MAUI applications.
/// </summary>
public static partial class HostBuilderExtensions
{
    /// <summary>
    /// Configures UXD Popups for MAUI applications.
    /// </summary>
    /// <param name="builder">The MAUI app builder.</param>
    /// <returns>The same builder instance for chaining.</returns>
    public static MauiAppBuilder UseUXDiversPopups(this MauiAppBuilder builder, bool closePopupOnBackAndroid = true)
    {
        // Set the current instances using the actual service provider from MAUI
        IPopupService.Current = PopupServiceCore.Instance;
        IPopupRegistryService.Current = PopupRegistryService.Instance;
        
        IPopupService.Current.Initialize(
            nativePopupManager: new NativePopupManager(),
            uiThreadDispatcher: new MauiUIThreadDispatcher(),
            viewModelAssignmentStrategy: new MauiViewModelAssignmentStrategy());

        // Register services with the DI container
        builder.Services.AddSingleton<IPopupService>(PopupServiceCore.Instance);
        builder.Services.AddSingleton<IPopupRegistryService>(PopupRegistryService.Instance);

        // Configure the registry service with the actual service provider function
        PopupRegistryService.Instance.UseServiceProvider(type => IPlatformApplication.Current?.Services.GetService(type));

#if ANDROID
        builder.AndroidSetup(closePopupOnBackAndroid);
#endif
        return builder;
    }
}