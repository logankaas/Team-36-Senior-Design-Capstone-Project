namespace SeniorCapstoneProject
{
    public partial class HealthJournalPage : ContentPage
    {
        private readonly string _userEmail;
        private readonly string _idToken;
        private string _selectedMood;

        public HealthJournalPage(string userEmail, string idToken)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            _userEmail = userEmail;
            _idToken = idToken;

            BindingContext = new { Today = DateTime.Now };
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private void OnMoodSelected(object sender, TappedEventArgs e)
        {
            if (e.Parameter is string mood)
            {
                _selectedMood = mood;

                // Reset all borders
                var parent = (sender as Border)?.Parent as Grid;
                if (parent != null)
                {
                    foreach (var child in parent.Children.OfType<Border>())
                    {
                        child.Stroke = Color.FromArgb("#E0E0E0");
                        child.StrokeThickness = 2;
                    }
                }

                // Highlight selected
                if (sender is Border selectedBorder)
                {
                    selectedBorder.Stroke = Color.FromArgb("#2E91FF");
                    selectedBorder.StrokeThickness = 3;
                }
            }
        }

        private void OnActivityClicked(object sender, TappedEventArgs e)
        {
            // Handle activity logging
            DisplayAlert("Activity", $"Logging {e.Parameter} activity", "OK");
        }

        private async void OnSaveEntryClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_selectedMood))
            {
                await DisplayAlert("Required", "Please select your mood", "OK");
                return;
            }

            var notes = NotesEditor.Text ?? "";

            // TODO: Save to Firestore
            await DisplayAlert("Saved", "Your health journal entry has been saved!", "OK");

            // Clear form
            NotesEditor.Text = "";
            _selectedMood = null;
            PainCheckBox.IsChecked = false;
            FatigueCheckBox.IsChecked = false;
            NauseaCheckBox.IsChecked = false;
            HeadacheCheckBox.IsChecked = false;
        }

        private async void OnViewEntryClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Coming Soon", "View past entries feature coming soon", "OK");
        }

        private async void OnHealthJournalClicked(object sender, EventArgs e)
        {
            // Already on this page
        }
    }
}