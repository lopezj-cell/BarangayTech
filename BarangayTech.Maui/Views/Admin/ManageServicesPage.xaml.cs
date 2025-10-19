using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace BarangayTech.Views.Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ManageServicesPage : ContentPage
    {
        public ManageServicesPage()
        {
            InitializeComponent();
        }

        private async void OnAddServiceClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Add Service", "Service creation form will be implemented here.", "OK");
        }

        private async void OnUpdateServiceClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Update Service", "Service update interface will be implemented here.", "OK");
        }

        private async void OnServiceRequestsClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Service Requests", "Service request management will be implemented here.", "OK");
        }
    }
}