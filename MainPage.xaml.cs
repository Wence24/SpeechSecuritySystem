using System;
using Microsoft.Maui.Controls;
using System.Timers;
using System.Threading.Tasks;

namespace SpeechSecuritySystem
{
    public partial class MainPage : ContentPage
    {
        private int failedAttempts = 0;
        private System.Timers.Timer? lockTimer;
        private TimeSpan remainingTime;
        private bool isLocked = false;
        private System.Timers.Timer? listeningTimer;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnSpeakButtonClicked(object sender, EventArgs e)
        {
            if (isLocked)
            {
                await DisplayAlert("Locked", $"Please wait {remainingTime.TotalSeconds} seconds before trying again.", "OK");
                return;
            }

            // Change button to microphone icon and adjust style
            SpeakButton.Text = ""; // Remove text
            SpeakButton.ImageSource = "microphone.png"; // Set to microphone icon
            SpeakButton.BackgroundColor = Colors.Transparent; // Make background transparent
            SpeakButton.WidthRequest = 60; // Make button smaller to fit microphone
            SpeakButton.HeightRequest = 60; // Set same height as width
            SpeakButton.IsEnabled = false;

            // Hide previous "No audio detected" message
            NoAudioMessage.IsVisible = false;

            // Start listening and timeout if no audio is detected
            bool isAudioDetected = await ListenForAudioAsync();

            if (isAudioDetected)
            {
                bool isVoiceRecognized = SimulateVoiceRecognition();

                if (isVoiceRecognized)
                {
                    await Navigation.PushAsync(new LandingPage());
                }
                else
                {
                    RegisterFailedAttempt();
                }
            }
            else
            {
                NoAudioMessage.IsVisible = true; // Show "No audio detected" message
                RegisterFailedAttempt();
            }

            // Revert button text and reset size
            SpeakButton.Text = "Speak to Unlock";
            SpeakButton.ImageSource = null; // Remove microphone icon
            SpeakButton.BackgroundColor = Colors.LightBlue; // Restore background color
            SpeakButton.WidthRequest = 200; // Reset width
            SpeakButton.HeightRequest = 60; // Reset height
            SpeakButton.IsEnabled = true;
        }

        private async Task<bool> ListenForAudioAsync()
        {
            bool audioDetected = false;

            // Start listening for 5 seconds
            listeningTimer = new System.Timers.Timer(5000); // 5 seconds
            listeningTimer.Elapsed += (s, e) => {
                listeningTimer.Stop();
                audioDetected = false; // No audio detected within the 5 seconds
            };

            listeningTimer.Start();

            // Simulate audio detection; replace this with actual audio detection logic
            await Task.Delay(3000); // Simulating delay; replace with actual audio check

            // If audio is detected within the 5-second period, stop the timer
            if (true /* replace with actual audio detection condition */)
            {
                audioDetected = true;
                listeningTimer.Stop();
            }

            return audioDetected;
        }

        private void RegisterFailedAttempt()
        {
            failedAttempts++;
            LockIcon.Source = "lock.png";
            ErrorMessage.IsVisible = true;
            AttemptStatusLabel.Text = $"Attempt {failedAttempts} of 3 failed";
            AttemptStatusLabel.IsVisible = true;

            if (failedAttempts >= 3)
            {
                SpeakButton.IsEnabled = false;
                SpeakButton.BackgroundColor = Colors.Gray;
                SpeakButton.Text = "Locked (Attempts exceeded)";
                StatusLabel.Text = "Voice Lock Inactive";
                StartLockTimer(TimeSpan.FromMinutes(1));
            }
            else
            {
                SpeakButton.IsEnabled = true;
                SpeakButton.BackgroundColor = Colors.LightBlue;
            }
        }

        private void StartLockTimer(TimeSpan duration)
        {
            isLocked = true;
            remainingTime = duration;
            TimerLabel.IsVisible = true;
            TimerLabel.Text = $"Time remaining: {remainingTime.Minutes}:{remainingTime.Seconds:D2}";

            lockTimer = new System.Timers.Timer(1000);
            lockTimer.Elapsed += OnTimerElapsed;
            lockTimer.Start();
        }

        private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            if (remainingTime.TotalSeconds > 0)
            {
                remainingTime = remainingTime.Subtract(TimeSpan.FromSeconds(1));
                MainThread.BeginInvokeOnMainThread(() => {
                    TimerLabel.Text = $"Time remaining: {remainingTime.Minutes}:{remainingTime.Seconds:D2}";
                });
            }
            else
            {
                lockTimer?.Stop();
                lockTimer?.Dispose();
                isLocked = false;
                failedAttempts = 0;

                MainThread.BeginInvokeOnMainThread(() => {
                    SpeakButton.IsEnabled = true;
                    SpeakButton.BackgroundColor = Colors.LightBlue;
                    SpeakButton.Text = "Speak to Unlock";
                    AttemptStatusLabel.IsVisible = false;
                    StatusLabel.Text = "Voice Lock Active";
                    ErrorMessage.IsVisible = false;
                    TimerLabel.IsVisible = false;
                    NoAudioMessage.IsVisible = false;
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
            SpeakButton.BackgroundColor = Colors.LightBlue;
            SpeakButton.Text = "Speak to Unlock";
            AttemptStatusLabel.Text = "Attempt 0 of 3 failed";
            AttemptStatusLabel.IsVisible = false;
            StatusLabel.Text = "Voice Lock Active";
            ErrorMessage.IsVisible = false;
            TimerLabel.IsVisible = false;
            NoAudioMessage.IsVisible = false;
        }

        private bool SimulateVoiceRecognition()
        {
            return new Random().Next(0, 0) < 2;
        }
    }
}
