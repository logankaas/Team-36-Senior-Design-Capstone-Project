using Microsoft.Maui.Controls;
using static SeniorCapstoneProject.App;

namespace SeniorCapstoneProject
{
    public partial class LandingPage : ContentPage
    {
        private readonly User _user;

        public LandingPage(User user)
        {
            InitializeComponent();
            _user = user;
            WelcomeLabel.Text = $"Welcome, {_user.FirstName}!";
        }

        private async void OnProfileClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProfilePage(_user));
        }

        private void OnChatClicked(object sender, EventArgs e)
        {
            // Navigate to chat page
        }

        private void OnHistoryClicked(object sender, EventArgs e)
        {
            // Navigate to history page
        }

        private void OnSettingsClicked(object sender, EventArgs e)
        {
            // Navigate to settings page
        }

        private void OnHelpClicked(object sender, EventArgs e)
        {
            // Navigate to help page
        }
    }
}