using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
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

        private async System.Threading.Tasks.Task PerformLogin(string username, string password)
        {
            try
            {
                // Show loading
                LoginButton.IsEnabled = false;
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;
                ErrorLabel.IsVisible = false;

                // Update UI with credentials
                UsernameEntry.Text = username;
                PasswordEntry.Text = password;

                // Perform login
                var result = await AuthService.LoginAsync(username, password);

                if (result.IsSuccess)
                {
                    // Navigate based on user role
                    await NavigateToMainApp(result.User);
                }
                else
                {
                    // Show error
                    ErrorLabel.Text = result.ErrorMessage;
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
            await DisplayAlert("Login Successful", $"Welcome, {user.FullName}!", "Continue");

            // Navigate to appropriate main page based on role
            if (user.Role == UserRole.Admin || user.Role == UserRole.SuperAdmin)
            {
                Application.Current.MainPage = new Views.Auth.AdminMainPage();
            }
            else
            {
                Application.Current.MainPage = new MainTabbedPage();
            }
        }

        private async void OnViewCredentialsClicked(object sender, EventArgs e)
        {
            var credentials = AuthService.GetSampleCredentials();
            var credentialText = "Sample Login Credentials:\n\n";

            credentialText += "ADMIN ACCOUNTS:\n";
            foreach (var user in credentials.Where(u => u.Role == UserRole.Admin || u.Role == UserRole.SuperAdmin))
            {
                credentialText += $"• {user.FullName}\n";
                credentialText += $"  Username: {user.Username}\n";
                credentialText += $"  Password: {user.Password}\n";
                credentialText += $"  Role: {user.Role}\n\n";
            }

            credentialText += "RESIDENT ACCOUNTS:\n";
            foreach (var user in credentials.Where(u => u.Role == UserRole.Resident))
            {
                credentialText += $"• {user.FullName}\n";
                credentialText += $"  Username: {user.Username}\n";
                credentialText += $"  Password: {user.Password}\n";
                if (!string.IsNullOrWhiteSpace(user.QuickSignKey))
                    credentialText += $"  Key: {user.QuickSignKey}\n";
                credentialText += "\n";
            }

            await DisplayAlert("Sample Credentials", credentialText, "OK");
        }

        // Firebase test handler - uses UsernameEntry as email
        private async void OnTestFirebaseSignInClicked(object sender, EventArgs e)
        {
            await TestFirebaseEmailSignIn(UsernameEntry.Text, PasswordEntry.Text);
        }

        // add inside LoginPage class
        private async System.Threading.Tasks.Task TestFirebaseEmailSignIn(string email, string password)
        {
            try
            {
                // Show loading
                FirebaseTestButton.IsEnabled = false;
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;
                ErrorLabel.IsVisible = false;

                var resp = await MobileApiService.FirebaseSignInWithEmailPasswordAsync(email, password);
                await DisplayAlert("Firebase Sign-in", $"OK. UID: {resp.LocalId}\nToken length: {(resp.IdToken?.Length ?? 0)}", "OK");
            }
            catch (Exception ex)
            {
                ErrorLabel.Text = ex.Message;
                ErrorLabel.IsVisible = true;
            }
            finally
            {
                FirebaseTestButton.IsEnabled = true;
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
            }
        }
    }
}