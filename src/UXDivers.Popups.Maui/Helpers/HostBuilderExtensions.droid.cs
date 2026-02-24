using Android.App;
using Android.OS;
using AndroidX.Activity;
using Microsoft.Maui.LifecycleEvents;
using UXDivers.Popups.Services;

namespace UXDivers.Popups.Maui;

public static partial class HostBuilderExtensions
{
    public static void AndroidSetup(this MauiAppBuilder builder, bool closePopupOnBackAndroid)
    {
        if (!closePopupOnBackAndroid)
        {
            return;
        }

        builder.ConfigureLifecycleEvents(events =>
        {
            events.AddAndroid(android =>
            {
                // Runs when the Android Activity is created
                android.OnCreate((activity, bundle) =>
                {
                    // MauiAppCompatActivity is a ComponentActivity -> has OnBackPressedDispatcher
                    if (activity is ComponentActivity componentActivity)
                    {
                        componentActivity.OnBackPressedDispatcher.AddCallback(
                            componentActivity,
                            new PopupBackCallback(componentActivity)
                        );
                    }
                });
            });
        });
    }

    private sealed class PopupBackCallback : OnBackPressedCallback
    {
        private readonly ComponentActivity _activity;

        public PopupBackCallback(ComponentActivity activity) : base(true)
        {
            _activity = activity;
        }

        public override void HandleOnBackPressed()
        {
            var stack = IPopupService.Current.NavigationStack;

            // If there is a popup, close it and consume Back
            if (stack.Count > 0)
            {
                _ = IPopupService.Current.PopAsync();
                return;
            }

            // No popups -> let the system/default navigation handle Back
            Enabled = false;
            _activity.OnBackPressedDispatcher.OnBackPressed();
            Enabled = true;
        }
    }
}