using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration;
using BarangayTech.Services;
using BarangayTech.Views.Auth;

namespace BarangayTech.Maui;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		// Configure Mobile API for the MAUI app (adjust baseUrl/apiKey as needed)
		MobileApiService.Configure(baseUrl: "http://10.0.2.2:5000", apiKey: null);

		// Start the app on the migrated LoginPage
		MainPage = new NavigationPage(new LoginPage());
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		// Ensure the window uses the application's MainPage
		return new Window(MainPage);
	}
}