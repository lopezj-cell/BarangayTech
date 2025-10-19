using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
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
                
                FullNameLabel.Text = user.FullName;
                PositionLabel.Text = user.Position ?? "Administrator";
                RoleLabel.Text = user.Role.ToString();
                EmailLabel.Text = user.Email;
                ContactLabel.Text = user.ContactNumber;
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
                Application.Current.MainPage = new NavigationPage(new LoginPage());
            }
        }
    }
}