using Microsoft.Maui.Controls;

namespace SpeechSecuritySystem
{
    public partial class LandingPage : ContentPage
    {
        public LandingPage()
        {
            InitializeComponent();
            Appearing += OnPageAppearing; // Subscribe to the Appearing event
        }

        private async void OnPageAppearing(object? sender, EventArgs e)
        {
            // Flash effect: Fade in the white BoxView and then fade it out
            await FlashBox.FadeTo(1, 300);  // Flash in
            await Task.Delay(200);          // Pause for a brief moment
            await FlashBox.FadeTo(0, 300);  // Flash out

            // Animate the Unlock Image and Welcome Text
            await Task.WhenAll(
                UnlockImage.FadeTo(1, 500), // Fade in the lock image
                WelcomeLabel.FadeTo(1, 500)  // Fade in the welcome text
            );

            // Pause for 3 seconds before fading out
            await Task.Delay(1000);

            // Fade out the Unlock Image and Welcome Text immediately
            await Task.WhenAll(
                UnlockImage.FadeTo(0, 500), // Fade out the lock image
                WelcomeLabel.FadeTo(0, 500)  // Fade out the welcome text
            );

            // Show the background image after the text and lock fade out
            // Set a background image for the landing page
            BackgroundImageSource = "apex.png"; // Background image
        }
    }
}
