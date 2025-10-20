using System;
using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using BarangayTech.Services.Auth;
using BarangayTech.Models;
using BarangayTech.Services;

namespace BarangayTech.Views.Auth
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            await PerformLogin(UsernameEntry.Text, PasswordEntry.Text);
        }

        private async void OnQuickAdminLoginClicked(object sender, EventArgs e)
        {
            await PerformLogin("admin", "admin123");
        }

        private async void OnQuickResidentLoginClicked(object sender, EventArgs e)
        {
            await PerformLogin("resident1", "resident123");
        }

        private async void OnQuickSuperAdminLoginClicked(object sender, EventArgs e)
        {
            await PerformLogin("superadmin", "super123");
        }

        private async System.Threading.Tasks.Task PerformLogin(string? username, string? password)
        {
            try
            {
                // Show loading
                LoginButton.IsEnabled = false;
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;
                ErrorLabel.IsVisible = false;

                // Update UI with credentials
                var usernameValue = username ?? string.Empty;
                var passwordValue = password ?? string.Empty;
                UsernameEntry.Text = usernameValue;
                PasswordEntry.Text = passwordValue;

                // Perform login
                var result = await AuthService.LoginAsync(usernameValue, passwordValue);

                if (result.IsSuccess && result.User != null)
                {
                    // Navigate based on user role
                    await NavigateToMainApp(result.User);
                }
                else
                {
                    // Show error
                    ErrorLabel.Text = result.ErrorMessage ?? "Invalid username or password.";
                    ErrorLabel.IsVisible = true;
                }
            }
            catch (Exception)
            {
                ErrorLabel.Text = "An error occurred during login. Please try again.";
                ErrorLabel.IsVisible = true;
            }
            finally
            {
                // Hide loading
                LoginButton.IsEnabled = true;
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
            }
        }

        private async System.Threading.Tasks.Task NavigateToMainApp(User user)
        {
            // Show success message
            var displayName = user.FullName ?? user.Username ?? "User";
            await DisplayAlert("Login Successful", $"Welcome, {displayName}!", "Continue");

            // Navigate to appropriate main page based on role
            if (user.Role == UserRole.Admin || user.Role == UserRole.SuperAdmin)
            {
                App.CurrentApp.SetRootPage(new AdminMainPage());
            }
            else
            {
                App.CurrentApp.SetRootPage(new MainTabbedPage());
            }
        }

        private async void OnViewCredentialsClicked(object sender, EventArgs e)
        {
            var credentialText = "To create an account:\n\n";
            credentialText += "1. Register through the app\n";
            credentialText += "2. Verify your email\n";
            credentialText += "3. Login with your credentials\n\n";
            credentialText += "For demo purposes, use the quick login buttons above.\n";
            credentialText += "(Note: These require actual Firebase accounts to be created first)";

            await DisplayAlert("Login Information", credentialText, "OK");
        }
    }
}