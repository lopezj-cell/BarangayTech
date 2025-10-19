using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using BarangayTech.Services;
using System.Threading.Tasks;

namespace BarangayTech.Views.Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdminTasksPage : ContentPage
    {
        public AdminTasksPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadTasks();
        }

        private async Task LoadTasks()
        {
            try
            {
                var data = await MobileApiService.GetAdminTasksAsync();
                TasksList.BindingContext = data;
            }
            catch
            {
                await DisplayAlert("Error", "Failed to load administrative tasks.", "OK");
            }
        }

        private async void OnAddClicked(object sender, EventArgs e)
        {
            var title = TitleEntry.Text?.Trim();
            if (string.IsNullOrWhiteSpace(title))
            {
                await DisplayAlert("Validation", "Please enter a task title.", "OK");
                return;
            }

            // Ask AI for suggestions
            var suggestion = await MobileApiService.SuggestTaskAsync(title, "");
            var accept = await DisplayAlert("AI Suggestion",
                $"Priority: {suggestion.SuggestedPriority}\nCategory: {suggestion.SuggestedCategory}\n\nUse these values?",
                "Use", "Edit");

            var priority = accept ? suggestion.SuggestedPriority : "Medium";
            var category = accept ? suggestion.SuggestedCategory : "General";

            var dto = new MobileApiService.AdministrativeTaskDto
            {
                Title = title,
                Description = "",
                Category = category,
                Priority = priority,
                Status = "Open",
                AssignedTo = "",
                DueDate = null
            };

            try
            {
                await MobileApiService.CreateAdminTaskAsync(dto);
                TitleEntry.Text = string.Empty;
                await LoadTasks();
            }
            catch
            {
                await DisplayAlert("Error", "Failed to create task.", "OK");
            }
        }

        private async void OnSuggestClicked(object sender, EventArgs e)
        {
            var title = TitleEntry.Text?.Trim();
            if (string.IsNullOrWhiteSpace(title))
            {
                await DisplayAlert("Validation", "Please enter a task title.", "OK");
                return;
            }
            try
            {
                var suggestion = await MobileApiService.SuggestTaskAsync(title, "");
                await DisplayAlert("AI Suggestion",
                    $"Suggested Priority: {suggestion.SuggestedPriority}\nSuggested Category: {suggestion.SuggestedCategory}",
                    "OK");
            }
            catch
            {
                await DisplayAlert("Error", "Failed to get suggestion.", "OK");
            }
        }

        private async void OnCompleteClicked(object sender, EventArgs e)
        {
            var dto = (sender as Button)?.CommandParameter as MobileApiService.AdministrativeTaskDto;
            if (dto == null) return;

            dto.Status = "Completed";
            try
            {
                await MobileApiService.UpdateAdminTaskAsync(dto.Id, dto);
                await LoadTasks();
            }
            catch
            {
                await DisplayAlert("Error", "Failed to update task.", "OK");
            }
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            var dto = (sender as Button)?.CommandParameter as MobileApiService.AdministrativeTaskDto;
            if (dto == null) return;

            var confirm = await DisplayAlert("Delete Task", "Are you sure you want to delete this task?", "Yes", "No");
            if (!confirm) return;

            try
            {
                await MobileApiService.DeleteAdminTaskAsync(dto.Id);
                await LoadTasks();
            }
            catch
            {
                await DisplayAlert("Error", "Failed to delete task.", "OK");
            }
        }
    }
}
