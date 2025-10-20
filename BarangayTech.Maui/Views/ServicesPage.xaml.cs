using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace BarangayTech.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ServicesPage : ContentPage
    {
        public ServicesPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                var data = await Services.MobileApiService.GetServicesAsync();
                BindingContext = data;
            }
            catch
            {
                await DisplayAlert("Error", "Failed to load services.", "OK");
            }
        }

        private async void OnCertificatesClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Certificates", "Certificate application form will be available here. You can apply for birth, death, and marriage certificates.", "OK");
        }

        private async void OnBusinessPermitClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Business Permit", "Business permit application process will be shown here. Required documents and fees will be listed.", "OK");
        }

        private async void OnHealthServicesClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Health Services", "Health center services and appointment booking will be available here.", "OK");
        }

        private async void OnSocialServicesClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Social Services", "Social assistance programs for senior citizens, PWDs, and other beneficiaries.", "OK");
        }

        private async void OnLegalServicesClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Legal Services", "Legal consultation and mediation services will be accessible here.", "OK");
        }

        private async void OnEmergencyServicesClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Emergency Services", "Emergency hotlines:\n\nBarangay Emergency: 123-4567\nFire Department: 116\nPolice: 117\nAmbulance: 911", "OK");
        }

        private async void OnWasteManagementClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Waste Management", "Garbage collection schedule and recycling programs information will be shown here.", "OK");
        }

        private async void OnPublicSafetyClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Public Safety", "Report security concerns and access peace and order information here.", "OK");
        }
    }
}