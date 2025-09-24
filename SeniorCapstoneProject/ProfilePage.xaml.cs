

namespace SeniorCapstoneProject
{
    public partial class ProfilePage : ContentPage
    {
        private readonly FirestoreService _firestoreService = new FirestoreService("seniordesigncapstoneproj-49cfd");

        private readonly UserDatabase _userDb;
        private User _user;

        public ProfilePage(User user)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "users.db3");
            _userDb = new UserDatabase(dbPath);
            _user = user;

            // Populate fields
            EmailEntry.Text = _user.Email;
            FirstNameEntry.Text = _user.FirstName;
            LastNameEntry.Text = _user.LastName;
            UsernameEntry.Text = _user.Username;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            var newEmail = EmailEntry.Text?.Trim() ?? "";
            var newFirstName = FirstNameEntry.Text?.Trim() ?? "";
            var newLastName = LastNameEntry.Text?.Trim() ?? "";
            var newUsername = UsernameEntry.Text?.Trim() ?? "";

            _user.Email = newEmail;
            _user.FirstName = newFirstName;
            _user.LastName = newLastName;
            _user.Username = newUsername;

            await _userDb.UpdateUserAsync(_user);

            var idToken = await SecureStorage.GetAsync("firebase_id_token");
            if (!string.IsNullOrEmpty(idToken))
            {
                await _firestoreService.SaveUserAsync(_user, idToken);
            }

            ProfileMessage.Text = "Profile updated!";
            ProfileMessage.TextColor = Colors.Green;
            ProfileMessage.IsVisible = true;
        }
    }
}