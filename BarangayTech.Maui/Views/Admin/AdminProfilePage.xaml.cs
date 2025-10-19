using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using BarangayTech.Services.Auth;
using BarangayTech.Views.Auth;

namespace BarangayTech.Views.Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdminProfilePage : ContentPage
    {
        public AdminProfilePage()
        {
            InitializeComponent();
            LoadUserProfile();
        }

        private void LoadUserProfile()
        {
            if (AuthService.CurrentUser != null)
            {
                var user = AuthService.CurrentUser;
                
                FullNameLabel.Text = user.FullName ?? user.Username ?? "Administrator";
                PositionLabel.Text = user.Position ?? "Administrator";
                RoleLabel.Text = user.Role.ToString();
                EmailLabel.Text = user.Email ?? "N/A";
                ContactLabel.Text = user.ContactNumber ?? "N/A";
                DepartmentLabel.Text = user.Department ?? "N/A";
                CreatedDateLabel.Text = user.CreatedDate.ToString("MMMM dd, yyyy");
                LastLoginLabel.Text = user.LastLoginDate.ToString("MMMM dd, yyyy hh:mm tt");
            }
        }

        private async void OnEditProfileClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Edit Profile", "Profile editing form will be implemented here.", "OK");
        }

        private async void OnChangePasswordClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Change Password", "Password change form will be implemented here.", "OK");
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