using System.Collections.Generic;

namespace SeniorCapstoneProject
{
    public partial class SymptomCheckerPage : ContentPage
    {
        private readonly string _userEmail;
        private readonly string _idToken;
        private List<string> _selectedSymptoms = new List<string>();
        private string _selectedBodyPart;

        public SymptomCheckerPage(string userEmail, string idToken)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            _userEmail = userEmail;
            _idToken = idToken;
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private void OnBodyPartSelected(object sender, TappedEventArgs e)
        {
            if (e.Parameter is string bodyPart)
            {
                _selectedBodyPart = bodyPart;

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

                DisplayAlert("Body Part Selected", $"You selected: {bodyPart}", "OK");
            }
        }

        private void OnSymptomSelected(object sender, TappedEventArgs e)
        {
            if (e.Parameter is string symptom)
            {
                if (_selectedSymptoms.Contains(symptom))
                {
                    _selectedSymptoms.Remove(symptom);

                    // Deselect visual
                    if (sender is Border border)
                    {
                        border.Background = Color.FromArgb("#FFFFFF");
                        border.Stroke = Color.FromArgb("#E0E0E0");
                    }
                }
                else
                {
                    _selectedSymptoms.Add(symptom);

                    // Select visual
                    if (sender is Border border)
                    {
                        border.Background = Color.FromArgb("#E3F2FD");
                        border.Stroke = Color.FromArgb("#2E91FF");
                        border.StrokeThickness = 2;
                    }
                }
            }
        }

        private async void OnAnalyzeClicked(object sender, EventArgs e)
        {
            if (_selectedSymptoms.Count == 0 && string.IsNullOrWhiteSpace(SymptomsEditor.Text))
            {
                await DisplayAlert("Input Required", "Please select symptoms or describe what you're experiencing", "OK");
                return;
            }

            string symptoms = string.Join(", ", _selectedSymptoms);
            if (!string.IsNullOrWhiteSpace(SymptomsEditor.Text))
            {
                symptoms += (symptoms.Length > 0 ? "; " : "") + SymptomsEditor.Text;
            }

            // TODO: Implement AI symptom analysis or database lookup
            await DisplayAlert("Analysis Complete",
                $"Based on your symptoms: {symptoms}\n\n" +
                "This is a placeholder. In production, this would analyze your symptoms and provide recommendations.\n\n" +
                "Remember: This is not a substitute for professional medical advice.",
                "OK");
        }

        private async void OnEmergencyCallClicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Emergency Call",
                "Are you sure you want to call 911?",
                "Call", "Cancel");

            if (answer)
            {
                try
                {
                    if (PhoneDialer.IsSupported)
                        PhoneDialer.Open("911");
                    else
                        await DisplayAlert("Error", "Phone dialer not supported on this device", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Unable to make call: {ex.Message}", "OK");
                }
            }
        }
    }
}