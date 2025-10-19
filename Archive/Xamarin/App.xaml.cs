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

            // configure MobileApiService with Firebase Web API key
            Services.MobileApiService.Configure(
                baseUrl: "http://10.0.2.2:5000",
                apiKey: null,
                firebaseApiKey: "AIzaSyAZtx2vDIHRTJ9fgbFJWT6hvkztcn2z5Wo" // <- replace with your key
            );

            MainPage = new NavigationPage(new LoginPage());
        }

        protected override void OnStart() { }
        protected override void OnSleep() { }
        protected override void OnResume() { }
    }
}