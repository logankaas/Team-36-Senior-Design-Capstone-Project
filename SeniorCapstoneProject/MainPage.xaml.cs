using System.IO;
using Microsoft.Maui.Controls;

namespace SeniorCapstoneProject
{
    public partial class MainPage : ContentPage
    {
        private readonly UserDatabase _userDb;

        public MainPage()
        {
            InitializeComponent();
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "users.db3");
            _userDb = new UserDatabase(dbPath);
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            var user = await _userDb.GetUserAsync(UsernameEntry.Text, PasswordEntry.Text);
            if (user != null)
            {
                LoginMessage.TextColor = Colors.Green;
                LoginMessage.Text = "Login successful!";
                LoginMessage.IsVisible = true;
                await Navigation.PushAsync(new LandingPage(user));
            }
            else
            {
                LoginMessage.TextColor = Colors.Red;
                LoginMessage.Text = "Invalid username or password.";
                LoginMessage.IsVisible = true;
            }
        }

        private async void OnCreateAccountClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NewAccountPage());
        }
    }
}