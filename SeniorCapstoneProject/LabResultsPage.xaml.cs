namespace SeniorCapstoneProject
{
    public partial class LabResultsPage : ContentPage
    {
        private readonly string _userEmail;
        private readonly string _idToken;

        public LabResultsPage(string userEmail, string idToken)
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

        private async void OnTestDetailClicked(object sender, EventArgs e)
        {
            string testName = "Lab Test";

            if (e is TappedEventArgs tappedArgs && tappedArgs.Parameter is string param)
            {
                testName = param;
            }

            await DisplayAlert("Test Details", $"Viewing details for {testName}", "OK");
        }

        private async void OnDownloadReportClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Coming Soon", "Download report feature coming soon", "OK");
        }
    }
}