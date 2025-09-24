using System.IO;
using Microsoft.Maui.Controls;

namespace SeniorCapstoneProject
{
    public partial class NewAccountPage : ContentPage
    {
        private readonly UserDatabase _userDb;

        private bool _isPasswordHidden = true;

        public bool IsPasswordHidden
        {
            get => _isPasswordHidden;
            set
            {
                _isPasswordHidden = value;
                RegisterPasswordEntry.IsPassword = value;
                PasswordToggleButton.Source = value ? "eyeslash.svg" : "eye.svg";
            }
        }

        public NewAccountPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            IsPasswordHidden = true;

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "users.db3");
            _userDb = new UserDatabase(dbPath);
        }

        private void OnPasswordToggleClicked(object sender, EventArgs e)
        {
            IsPasswordHidden = !IsPasswordHidden;
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            string email = RegisterEmailEntry.Text?.Trim() ?? string.Empty;
            string firstName = RegisterFirstNameEntry.Text?.Trim() ?? string.Empty;
            string lastName = RegisterLastNameEntry.Text?.Trim() ?? string.Empty;
            string username = RegisterUsernameEntry.Text?.Trim() ?? string.Empty;
            string password = RegisterPasswordEntry.Text ?? string.Empty;

            RegisterMessage.IsVisible = false;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(firstName) ||
                string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                RegisterMessage.Text = "Please fill in all fields.";
                RegisterMessage.TextColor = Colors.Red;
                RegisterMessage.IsVisible = true;
                return;
            }

            var existingUser = await _userDb.GetUserByUsernameAsync(username);
            if (existingUser != null)
            {
                RegisterMessage.Text = "User already exists.";
                RegisterMessage.TextColor = Colors.Red;
                RegisterMessage.IsVisible = true;
                return;
            }

            var user = new User
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Username = username,
                Password = password
            };
            await _userDb.AddUserAsync(user);

            RegisterMessage.Text = "User registered successfully!";
            RegisterMessage.TextColor = Colors.Green;
            RegisterMessage.IsVisible = true;

            RegisterEmailEntry.Text = string.Empty;
            RegisterFirstNameEntry.Text = string.Empty;
            RegisterLastNameEntry.Text = string.Empty;
            RegisterUsernameEntry.Text = string.Empty;
            RegisterPasswordEntry.Text = string.Empty;
        }

        private async void OnBackToLoginClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}