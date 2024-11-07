using Microsoft.Maui.Controls;

namespace SpeechSecuritySystem
{
    public partial class LandingPage : ContentPage
    {
        public LandingPage()
        {
            InitializeComponent();
            Appearing += OnPageAppearing;

            // Ensure login system is hidden initially
            LoginContainer.IsVisible = false;
        }

        private async void OnPageAppearing(object? sender, EventArgs e)
        {
            // Hide login system and show welcome elements
            LoginContainer.IsVisible = false;
            WelcomeStack.IsVisible = true;

            // Flash effect: Fade in the white BoxView and then fade it out
            await FlashBox.FadeTo(1, 300);  // Flash in
            await Task.Delay(200);          // Pause for a brief moment
            await FlashBox.FadeTo(0, 300);  // Flash out

            // Animate the Unlock Image and Welcome Text
            await Task.WhenAll(
                UnlockImage.FadeTo(1, 500), // Fade in the lock image
                WelcomeLabel.FadeTo(1, 500)  // Fade in the welcome text
            );

            // Pause before fading out
            await Task.Delay(1000);

            // Fade out the Welcome elements
            await Task.WhenAll(
                UnlockImage.FadeTo(0, 500),
                WelcomeLabel.FadeTo(0, 500)
            );

            // Set background image
            BackgroundImageSource = "comebg.png";

            // Hide welcome stack and show login system with fade
            WelcomeStack.IsVisible = false;
            LoginContainer.Opacity = 0;
            LoginContainer.IsVisible = true;
            await LoginContainer.FadeTo(1, 500);
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string username = UsernameEntry.Text;
            string password = PasswordEntry.Text;

            if (username == "admin" && password == "admin123")
            {
                await Navigation.PushAsync(new AdminDashboard());
            }
            else
            {
                await DisplayAlert("Login Failed", "Invalid username or password. Please try again.", "OK");
            }
        }
    }
}