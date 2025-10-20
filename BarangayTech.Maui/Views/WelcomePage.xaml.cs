using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

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
            // Swap the root page to the tabbed experience for residents
            App.CurrentApp.SetRootPage(new MainTabbedPage());
        }
    }
}