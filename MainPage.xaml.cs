using System;
using Microsoft.Maui.Controls;
using System.Timers; // Ensure to use System.Timers

namespace SpeechSecuritySystem
{
    public partial class MainPage : ContentPage
    {
        private int failedAttempts = 0;
        private System.Timers.Timer? lockTimer; // Declare as nullable
        private TimeSpan remainingTime;
        private bool isLocked = false;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnSpeakButtonClicked(object sender, EventArgs e)
        {
            // Disable the button if locked
            if (isLocked)
            {
                await DisplayAlert("Locked", $"Please wait {remainingTime.TotalSeconds} seconds before trying again.", "OK");
                return;
            }

            SpeakButton.IsEnabled = false;

            // Simulate voice recognition
            bool isVoiceRecognized = SimulateVoiceRecognition(); // Replace with your actual recognition logic

            if (isVoiceRecognized)
            {
                await Navigation.PushAsync(new LandingPage());
            }
            else
            {
                failedAttempts++;
                LockIcon.Source = "lock.png";
                ErrorMessage.IsVisible = true;
                AttemptStatusLabel.Text = $"Attempt {failedAttempts} of 5 failed";
                AttemptStatusLabel.IsVisible = true;

                if (failedAttempts >= 5)
                {
                    SpeakButton.IsEnabled = false;
                    SpeakButton.BackgroundColor = Colors.Gray;
                    SpeakButton.Text = "Locked (5 attempts exceeded)";
                    StatusLabel.Text = "Voice Lock Inactive"; // Change label text

                    // Start the timer based on whether it is the first exceedance or a subsequent one
                    StartLockTimer(isLocked ? TimeSpan.FromMinutes(10) : TimeSpan.FromMinutes(1));
                }
                else
                {
                    SpeakButton.IsEnabled = true;
                    SpeakButton.BackgroundColor = Colors.LightBlue;
                }
            }
        }

        private void StartLockTimer(TimeSpan duration)
        {
            isLocked = true;
            remainingTime = duration;
            TimerLabel.IsVisible = true; // Show the timer label
            TimerLabel.Text = $"Time remaining: {remainingTime.Minutes}:{remainingTime.Seconds:D2}"; // Set initial time

            lockTimer = new System.Timers.Timer(1000); // 1 second intervals
            lockTimer.Elapsed += OnTimerElapsed; // Attach the elapsed event
            lockTimer.Start(); // Start the timer
        }

        private void OnTimerElapsed(object? sender, ElapsedEventArgs e) // Use nullable parameter
        {
            if (remainingTime.TotalSeconds > 0)
            {
                remainingTime = remainingTime.Subtract(TimeSpan.FromSeconds(1));

                // Update the timer label on the main thread
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    TimerLabel.Text = $"Time remaining: {remainingTime.Minutes}:{remainingTime.Seconds:D2}";
                });
            }
            else
            {
                // Timer completed, unlock
                lockTimer?.Stop(); // Safely stop the timer if it's not null
                lockTimer?.Dispose(); // Dispose of the timer to release resources
                isLocked = false;
                failedAttempts = 0; // Reset failed attempts

                // Update UI on the main thread
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    SpeakButton.IsEnabled = true;
                    SpeakButton.BackgroundColor = Colors.LightBlue;
                    SpeakButton.Text = "Speak to Unlock";
                    AttemptStatusLabel.IsVisible = false; // Hide the attempt status label
                    StatusLabel.Text = "Voice Lock Active"; // Change back to active
                    ErrorMessage.IsVisible = false; // Reset error message visibility
                    TimerLabel.IsVisible = false; // Hide the timer when done
                });
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ResetAll();
        }

        private void ResetAll()
        {
            failedAttempts = 0;
            SpeakButton.IsEnabled = true;
            SpeakButton.BackgroundColor = Colors.LightBlue; // Ensure button starts active
            SpeakButton.Text = "Speak to Unlock";
            AttemptStatusLabel.Text = "Attempt 0 of 5 failed"; // Reset attempt status
            AttemptStatusLabel.IsVisible = false; // Hide the attempt status label initially
            StatusLabel.Text = "Voice Lock Active"; // Set initial state
            ErrorMessage.IsVisible = false; // Reset error message visibility
            TimerLabel.IsVisible = false; // Hide timer initially
        }

        // Simulated voice recognition (replace with your actual logic)
        private bool SimulateVoiceRecognition()
        {
            // Simulate a failure 80% of the time for testing purposes
            return new Random().Next(0, 10) < 2; // 20% chance of success
        }
    }
}
