using Microsoft.Maui.Controls;
using Microsoft.Maui.Layouts;

namespace SpeechSecuritySystem
{
    public partial class AdminDashboard : ContentPage
    {
        public AdminDashboard()
        {
            InitializeComponent();
        }

        private async void OnManageUsersButtonClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Manage Users", "Navigating to manage users page...", "OK");
        }

        private async void OnSystemSettingsButtonClicked(object sender, EventArgs e)
        {
            await DisplayAlert("System Settings", "Navigating to system settings page...", "OK");
        }

        private async void OnLogoutButtonClicked(object sender, EventArgs e)
        {
            // Disable the logout button to prevent multiple clicks
            LogoutButton.IsEnabled = false;

            try
            {
                // Create an AbsoluteLayout to overlay everything
                var overlayLayout = new AbsoluteLayout
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill
                };

                // Create and add a white overlay for the flash effect
                var flashOverlay = new BoxView
                {
                    Color = Colors.White,
                    Opacity = 0
                };
                AbsoluteLayout.SetLayoutFlags(flashOverlay, AbsoluteLayoutFlags.All);
                AbsoluteLayout.SetLayoutBounds(flashOverlay, new Rect(0, 0, 1, 1));

                // Create a vertical stack layout for the image and text
                var logoutStack = new VerticalStackLayout
                {
                    Spacing = 20,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Opacity = 0
                };

                // Create the COMELEC image
                var comelecImage = new Image
                {
                    Source = "comelec.png",
                    WidthRequest = 200,
                    HeightRequest = 200,
                    HorizontalOptions = LayoutOptions.Center
                };

                // Create the "Logging Out" label
                var loggingOutLabel = new Label
                {
                    Text = "Logging Out",
                    TextColor = Colors.Black,
                    FontSize = 28,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Center
                };

                // Add image and label to the stack
                logoutStack.Children.Add(comelecImage);
                logoutStack.Children.Add(loggingOutLabel);

                // Add the stack to the AbsoluteLayout
                AbsoluteLayout.SetLayoutFlags(logoutStack, AbsoluteLayoutFlags.PositionProportional);
                AbsoluteLayout.SetLayoutBounds(logoutStack, new Rect(0.5, 0.5, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

                // Add elements to the overlay layout
                overlayLayout.Children.Add(flashOverlay);
                overlayLayout.Children.Add(logoutStack);

                // Add the overlay layout to the page
                var mainGrid = this.Content as Grid;
                if (mainGrid != null)
                {
                    mainGrid.Children.Add(overlayLayout);

                    // Make sure the overlay is on top
                    Grid.SetRowSpan(overlayLayout, Grid.GetRowSpan(mainGrid));
                    Grid.SetColumnSpan(overlayLayout, Grid.GetColumnSpan(mainGrid));

                    // Initial flash effect
                    await flashOverlay.FadeTo(0.9, 200);
                    await flashOverlay.FadeTo(1, 200);

                    // Show the logout stack with animation
                    await logoutStack.FadeTo(1, 400, Easing.CubicOut);

                    // Hold the message briefly
                    await Task.Delay(1500);

                    // Fade out effect
                    await Task.WhenAll(
                        flashOverlay.FadeTo(1, 500),
                        logoutStack.FadeTo(0, 500)
                    );

                    // Navigate to MainPage
                    await Navigation.PopToRootAsync();
                }
                else
                {
                    await Navigation.PopToRootAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Logout error: {ex.Message}");
                await DisplayAlert("Error", "There was an error during logout. Returning to main page.", "OK");
                await Navigation.PopToRootAsync();
            }
            finally
            {
                // Re-enable the logout button
                LogoutButton.IsEnabled = true;
            }
        }
    }
}