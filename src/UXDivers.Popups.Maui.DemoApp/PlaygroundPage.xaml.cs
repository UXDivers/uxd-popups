using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UXDivers.Popups.Services;

namespace UXDivers.Popups.Maui.DemoApp;

public partial class PlaygroundPage : PopupPage
{
    public static readonly BindableProperty CloseButtonCommandProperty = BindableProperty.Create(
        nameof(CloseButtonCommand),
        typeof(ICommand),
        typeof(PlaygroundPage),
        defaultValueCreator: _ => new Command(async () => await IPopupService.Current.PopAsync()));
        
    public ICommand CloseButtonCommand
    {
        get { return (ICommand)GetValue(CloseButtonCommandProperty); }
        set { SetValue(CloseButtonCommandProperty, value); }
    }
    
    public PlaygroundPage()
    {
        InitializeComponent();

        inAnimationPicker.ItemsSource = new List<string>
        {
            "FadeInPopupAnimation",
            "AppearingPopupAnimation",
            "MoveInPopupAnimation",
            "ScaleInPopupAnimation"
        };
        inAnimationPicker.SelectedIndex = 0;

        outAnimationPicker.ItemsSource = new List<string>
        {
            "FadeOutPopupAnimation",
            "DisappearingPopupAnimation",
            "MoveOutPopupAnimation",
            "ScaleOutPopupAnimation"
        };
        outAnimationPicker.SelectedIndex = 0;

        overlayOpacity.Value = 100;
    }
    
    private async void OnTestClicked(object sender, EventArgs e)
    {
        IBaseAnimation appearing = inAnimationPicker.SelectedItem switch
        {
            "FadeInPopupAnimation" => new FadeInPopupAnimation { Duration = 500 },
            "AppearingPopupAnimation" => new AppearingPopupAnimation(),
            "MoveInPopupAnimation" => new MoveInPopupAnimation { Duration = 500 },
            "ScaleInPopupAnimation" => new ScaleInPopupAnimation { Duration = 500 },
            _ => new FadeInPopupAnimation { Duration = 500 },
        };

        IBaseAnimation disappearing = outAnimationPicker.SelectedItem switch
        {
            "FadeOutPopupAnimation" => new FadeOutPopupAnimation { Duration = 500 },
            "DisappearingPopupAnimation" => new DisappearingPopupAnimation(),
            "MoveOutPopupAnimation" => new MoveOutPopupAnimation { Duration = 500 },
            "ScaleOutPopupAnimation" => new ScaleOutPopupAnimation { Duration = 500 },
            _ => new FadeOutPopupAnimation { Duration = 500 },
        };

        var popup = new TestPopup()
        {
            AppearingAnimation = appearing,
            DisappearingAnimation = disappearing,
            CloseWhenBackgroundIsClicked = closeOnBackgroundClicked.IsToggled,
            PreventBackButtonDismiss = preventBackButtonDismiss.IsToggled,
            BackgroundInputTransparent = backgroundInputTransparent.IsToggled,
            BackgroundOpacity = overlayOpacity.Value / 100,
            CloseButtonIconText = MaterialSymbolsFont.Close,
            CloseButtonIconColor = Application.Current?.Resources["TextColor"] as Color ?? Colors.White,
            IconText = MaterialSymbolsFont.Check_circle,
            IconColor = Application.Current?.Resources["IconBlue"] as Color ?? Colors.Blue,
            Title = "Title Goes Here",
            Text = "Add some text here",
        };
        
        await IPopupService.Current.PushAsync(popup);
    }
}