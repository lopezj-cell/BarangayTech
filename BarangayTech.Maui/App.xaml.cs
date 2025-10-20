using Microsoft.Maui;
using Microsoft.Maui.Controls;
using BarangayTech.Services;
using BarangayTech.Views.Auth;

namespace BarangayTech;

public partial class App : Application
{
	private Window? _mainWindow;

	public App()
	{
		InitializeComponent();

		// Configure REST client defaults at startup; API key optional
		MobileApiService.Configure(baseUrl: "http://10.0.2.2:5000");
	}

	public static App CurrentApp => (App)(Current ?? throw new InvalidOperationException("Application not initialized."));

	protected override Window CreateWindow(IActivationState? activationState)
	{
		_mainWindow = new Window
		{
			Page = new NavigationPage(new LoginPage())
		};

		return _mainWindow;
	}

	public void SetRootPage(Page page, bool wrapInNavigation = false)
	{
		if (_mainWindow == null && Windows.Count > 0)
		{
			_mainWindow = Windows[0];
		}

		if (_mainWindow == null)
		{
			return;
		}

		_mainWindow.Page = wrapInNavigation ? new NavigationPage(page) : page;
	}
}