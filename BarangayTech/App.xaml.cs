using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using BarangayTech.Views;

namespace BarangayTech
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Configure mobile API + Firebase API key (use the key from your google-services.json)
            Services.MobileApiService.Configure(
                baseUrl: "http://10.0.2.2:5000", 
                apiKey: null,
                firebaseApiKey: "AIzaSyAZtx2vDIHRTJ9fgbFJWT6hvkztcn2z5Wo"
            );

            MainPage = new NavigationPage(new LoginPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}

