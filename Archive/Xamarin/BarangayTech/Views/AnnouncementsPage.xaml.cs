using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BarangayTech.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AnnouncementsPage : ContentPage
    {
        public AnnouncementsPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                var data = await Services.MobileApiService.GetAnnouncementsAsync();
                BindingContext = data;
            }
            catch (Exception)
            {
                await DisplayAlert("Error", "Failed to load announcements.", "OK");
            }
        }

        private async void OnAnnouncementDetailsClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Announcement Details", "Detailed information about this announcement will be displayed here.", "OK");
        }

        private async void OnSummaryClicked(object sender, EventArgs e)
        {
            try
            {
                // Concatenate latest announcements' text to summarize
                var items = BindingContext as System.Collections.IEnumerable;
                string combined = "";
                if (items != null)
                {
                    foreach (var it in items)
                    {
                        var titleProp = it.GetType().GetProperty("Title");
                        var descProp = it.GetType().GetProperty("Content") ?? it.GetType().GetProperty("Description");
                        var title = titleProp?.GetValue(it)?.ToString();
                        var desc = descProp?.GetValue(it)?.ToString();
                        if (!string.IsNullOrWhiteSpace(title)) combined += title + ". ";
                        if (!string.IsNullOrWhiteSpace(desc)) combined += desc + " ";
                    }
                }
                if (string.IsNullOrWhiteSpace(combined))
                {
                    AiOutputLabel.Text = "No announcements to summarize.";
                    return;
                }
                var summary = await Services.MobileApiService.SummarizeAsync(combined);
                AiOutputLabel.Text = summary;
            }
            catch
            {
                await DisplayAlert("Error", "Failed to summarize.", "OK");
            }
        }

        private async void OnTranslateClicked(object sender, EventArgs e)
        {
            try
            {
                var text = AiOutputLabel.Text;
                if (string.IsNullOrWhiteSpace(text))
                {
                    await DisplayAlert("Info", "Generate a summary first.", "OK");
                    return;
                }
                var translated = await Services.MobileApiService.TranslateAsync(text, "fil");
                AiOutputLabel.Text = translated;
            }
            catch
            {
                await DisplayAlert("Error", "Failed to translate.", "OK");
            }
        }
    }
}