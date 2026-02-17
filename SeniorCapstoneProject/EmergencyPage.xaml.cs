namespace SeniorCapstoneProject
{
    public partial class EmergencyPage : ContentPage
    {
        private readonly string _userEmail;
        private readonly string _idToken;

        public EmergencyPage(string userEmail, string idToken)
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

        private async void OnCall911Clicked(object sender, EventArgs e)
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

        private async void OnEditMedicalIdClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Coming Soon", "Medical ID editing will be available soon", "OK");
        }

        private async void OnCallContactClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button?.Parent?.Parent is Grid grid)
            {
                // Find the phone number label in the grid
                var phoneLabel = grid.Children
                    .OfType<VerticalStackLayout>()
                    .FirstOrDefault()?
                    .Children
                    .OfType<Label>()
                    .FirstOrDefault(l => l.Text.StartsWith("+1"));

                if (phoneLabel != null)
                {
                    try
                    {
                        if (PhoneDialer.IsSupported)
                            PhoneDialer.Open(phoneLabel.Text);
                        else
                            await DisplayAlert("Error", "Phone dialer not supported", "OK");
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", $"Unable to make call: {ex.Message}", "OK");
                    }
                }
            }
        }

        private async void OnAddContactClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Coming Soon", "Add emergency contact feature coming soon", "OK");
        }

        private async void OnGetDirectionsClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Coming Soon", "Map directions feature coming soon", "OK");
        }
    }
}