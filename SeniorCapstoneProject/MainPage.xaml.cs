using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace SeniorCapstoneProject
{
    public partial class MainPage : ContentPage
    {
        private readonly FirebaseAuthService _firebaseAuthService = new FirebaseAuthService("AIzaSyA_WhqRi9PKiFcsswW543zMBTr3OFyQsLs");
        private readonly FirestoreService _firestoreService = new FirestoreService("seniordesigncapstoneproj-49cfd");

        public MainPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string email = EmailEntry.Text?.Trim() ?? string.Empty;
            string password = PasswordEntry.Text ?? string.Empty;

            LoginMessage.IsVisible = false;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                LoginMessage.Text = "Please enter email and password.";
                LoginMessage.TextColor = Colors.Red;
                LoginMessage.IsVisible = true;
                return;
            }

            var firebaseIdToken = await _firebaseAuthService.SignInAsync(email, password);
            if (firebaseIdToken == null)
            {
                LoginMessage.Text = "Login failed. Check your credentials.";
                LoginMessage.TextColor = Colors.Red;
                LoginMessage.IsVisible = true;
                return;
            }

            await SecureStorage.SetAsync("firebase_id_token", firebaseIdToken);
            await SecureStorage.SetAsync("user_email", email);

            var (user, docId) = await _firestoreService.GetUserByEmailAsync(email, firebaseIdToken);
            if (user == null || docId == null)
            {
                LoginMessage.Text = "User profile not found in Firestore. Please try again or create a new account.";
                LoginMessage.TextColor = Colors.Red;
                LoginMessage.IsVisible = true;
                return;
            }

            await Navigation.PushAsync(new LandingPage(user));
        }

        private async void OnForgotPasswordClicked(object sender, EventArgs e)
        {
            string email = EmailEntry.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(email))
            {
                LoginMessage.Text = "Enter your email to reset password.";
                LoginMessage.TextColor = Colors.Red;
                LoginMessage.IsVisible = true;
                return;
            }

            bool sent = await _firebaseAuthService.SendPasswordResetEmailAsync(email);
            if (sent)
            {
                LoginMessage.Text = "Password reset email sent. Check your inbox.";
                LoginMessage.TextColor = Colors.Green;
                LoginMessage.IsVisible = true;
            }
            else
            {
                LoginMessage.Text = "Failed to send reset email. Is the email correct?";
                LoginMessage.TextColor = Colors.Red;
                LoginMessage.IsVisible = true;
            }
        }

        private async void OnCreateAccountClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NewAccountPage());
        }
    }
}