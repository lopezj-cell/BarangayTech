using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BarangayTech.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WelcomePage : ContentPage
    {
        public WelcomePage()
        {
            InitializeComponent();
        }

        private void OnGetStartedClicked(object sender, EventArgs e)
        {
            // Navigate to the main tabbed page
            Application.Current.MainPage = new MainTabbedPage();
        }
    }
}