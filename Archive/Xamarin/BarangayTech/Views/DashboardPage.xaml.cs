using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
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
            Device.StartTimer(TimeSpan.FromMinutes(1), () =>
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
                Application.Current.MainPage = new NavigationPage(new LoginPage());
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
                    await DisplayAlert("Profile", $"Name: {AuthService.CurrentUser.FullName}\nRole: {AuthService.CurrentUser.Role}\nEmail: {AuthService.CurrentUser.Email}", "OK");
                }
                else if (action == "Logout")
                {
                    var result = await DisplayAlert("Logout", "Are you sure you want to logout?", "Yes", "No");
                    if (result)
                    {
                        AuthService.Logout();
                        Application.Current.MainPage = new NavigationPage(new LoginPage());
                    }
                }
            }
        }
    }
}