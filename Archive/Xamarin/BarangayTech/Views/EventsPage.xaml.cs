using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BarangayTech.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EventsPage : ContentPage
    {
        public EventsPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                var data = await Services.MobileApiService.GetEventsAsync();
                BindingContext = data;
            }
            catch
            {
                await DisplayAlert("Error", "Failed to load events.", "OK");
            }
        }

        private async void OnEventDetailsClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Event Details", "Detailed information about the Community Clean-up Drive will be displayed here.", "OK");
        }

        private async void OnEventRegisterClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Register", "Registration form for the Health and Wellness Seminar will be shown here.", "OK");
        }

        private async void OnEventJoinClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Join Celebration", "Information about joining the Christmas Festival will be displayed here.", "OK");
        }
    }
}