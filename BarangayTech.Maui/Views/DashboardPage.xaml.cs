using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using BarangayTech.Services.Auth;
using BarangayTech.Views.Auth;

namespace BarangayTech.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DashboardPage : ContentPage
    {
        public DashboardPage()
        {
            InitializeComponent();
            UpdateDateTime();
            
            // Update time every minute
            Dispatcher.StartTimer(TimeSpan.FromMinutes(1), () =>
            {
                UpdateDateTime();
                return true;
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            // Check if user is logged in
            if (!AuthService.IsLoggedIn)
            {
                App.CurrentApp.SetRootPage(new LoginPage(), wrapInNavigation: true);
            }
        }

        private void UpdateDateTime()
        {
            DateTimeLabel.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy - hh:mm tt");
        }

        private async void OnReportIssueClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Report Issue", "This feature will allow residents to report community issues.", "OK");
        }

        private async void OnEmergencyClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Emergency", "Emergency contacts and procedures will be displayed here.", "OK");
        }

        private async void OnContactUsClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Contact Us", "Barangay contact information will be shown here.", "OK");
        }

        private async void OnProfileClicked(object sender, EventArgs e)
        {
    		if (AuthService.CurrentUser != null)
            {
                var action = await DisplayActionSheet("Profile Options", "Cancel", null, "View Profile", "Logout");
                
                if (action == "View Profile")
                {
    				var currentUser = AuthService.CurrentUser;
    				var name = currentUser.FullName ?? currentUser.Username ?? "Resident";
    				var email = currentUser.Email ?? "N/A";
    				await DisplayAlert("Profile", $"Name: {name}\nRole: {currentUser.Role}\nEmail: {email}", "OK");
                }
                else if (action == "Logout")
                {
                    var result = await DisplayAlert("Logout", "Are you sure you want to logout?", "Yes", "No");
                    if (result)
                    {
                        AuthService.Logout();
                        App.CurrentApp.SetRootPage(new LoginPage(), wrapInNavigation: true);
                    }
                }
            }
        }
    }
}