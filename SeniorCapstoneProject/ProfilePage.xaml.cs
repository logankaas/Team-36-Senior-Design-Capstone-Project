using System.IO;
using Microsoft.Maui.Controls;

namespace SeniorCapstoneProject
{
    public partial class ProfilePage : ContentPage
    {
        private readonly UserDatabase _userDb;
        private User _user;

        public ProfilePage(User user)
        {
            InitializeComponent();
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "users.db3");
            _userDb = new UserDatabase(dbPath);
            _user = user;

            // Populate fields
            EmailEntry.Text = _user.Email;
            FirstNameEntry.Text = _user.FirstName;
            LastNameEntry.Text = _user.LastName;
            UsernameEntry.Text = _user.Username;
            PasswordEntry.Text = _user.Password;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            _user.Email = EmailEntry.Text?.Trim() ?? "";
            _user.FirstName = FirstNameEntry.Text?.Trim() ?? "";
            _user.LastName = LastNameEntry.Text?.Trim() ?? "";
            _user.Username = UsernameEntry.Text?.Trim() ?? "";
            _user.Password = PasswordEntry.Text ?? "";

            await _userDb.UpdateUserAsync(_user);

            ProfileMessage.Text = "Profile updated!";
            ProfileMessage.TextColor = Colors.Green;
            ProfileMessage.IsVisible = true;
        }
    }
}