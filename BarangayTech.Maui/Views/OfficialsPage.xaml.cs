using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace BarangayTech.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OfficialsPage : ContentPage
    {
        public OfficialsPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                var data = await Services.MobileApiService.GetOfficialsAsync();
                BindingContext = data;
            }
            catch
            {
                await DisplayAlert("Error", "Failed to load officials.", "OK");
            }
        }

        private async void OnContactOfficialClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var frame = button?.Parent as StackLayout;
            
            await DisplayAlert("Contact Official", "Contact options:\n\n• Phone Call\n• Email\n• Schedule Appointment\n• Send Message", "OK");
        }
    }
}