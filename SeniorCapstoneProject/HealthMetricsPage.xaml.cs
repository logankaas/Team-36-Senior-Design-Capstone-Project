namespace SeniorCapstoneProject
{
    public partial class HealthMetricsPage : ContentPage
    {
        private readonly string _userEmail;
        private readonly string _idToken;

        public HealthMetricsPage(string userEmail, string idToken)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            _userEmail = userEmail;
            _idToken = idToken;

            LoadHealthMetrics();
        }

        private void LoadHealthMetrics()
        {
            // TODO: Load from Firestore
            BindingContext = new
            {
                Steps = "8,234",
                WaterIntake = "6",
                HeartRate = "72"
            };
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnLogReadingClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Coming Soon", "Log new health reading feature coming soon", "OK");
        }

        private async void OnViewHistoryClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Coming Soon", "View health history feature coming soon", "OK");
        }
    }
}