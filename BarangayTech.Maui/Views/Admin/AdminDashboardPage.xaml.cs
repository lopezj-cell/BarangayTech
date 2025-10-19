using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
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
            Dispatcher.StartTimer(TimeSpan.FromMinutes(1), () =>
            {
                UpdateDateTime();
                return true;
            });
        }

        private void LoadUserInfo()
        {
            if (AuthService.CurrentUser != null)
            {
                var user = AuthService.CurrentUser;
                var name = user.FullName ?? user.Username ?? "Administrator";
                WelcomeLabel.Text = $"Welcome, {name}";
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
                App.CurrentApp.SetRootPage(new LoginPage(), wrapInNavigation: true);
            }
        }
    }
}