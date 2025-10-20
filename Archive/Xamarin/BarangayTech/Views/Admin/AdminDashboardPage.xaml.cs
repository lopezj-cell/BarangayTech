using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using BarangayTech.Services.Auth;
using BarangayTech.Views.Auth;

namespace BarangayTech.Views.Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdminDashboardPage : ContentPage
    {
        public AdminDashboardPage()
        {
            InitializeComponent();
            LoadUserInfo();
            UpdateDateTime();
            
            // Update time every minute
            Device.StartTimer(TimeSpan.FromMinutes(1), () =>
            {
                UpdateDateTime();
                return true;
            });
        }

        private void LoadUserInfo()
        {
            if (AuthService.CurrentUser != null)
            {
                WelcomeLabel.Text = $"Welcome, {AuthService.CurrentUser.FullName}";
            }
        }

        private void UpdateDateTime()
        {
            DateTimeLabel.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy - hh:mm tt");
        }

        private async void OnAddEventClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Add Event", "Event creation form will be available here.", "OK");
        }

        private async void OnAddAnnouncementClicked(object sender, EventArgs e)
        {
            await DisplayAlert("New Announcement", "Announcement creation form will be available here.", "OK");
        }

        private async void OnManageUsersClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Manage Users", "User management interface will be available here.", "OK");
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
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