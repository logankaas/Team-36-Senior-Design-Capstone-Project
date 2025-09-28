using Microsoft.Maui.Storage;
using System.IO;

namespace SeniorCapstoneProject
{
    public partial class LoadingPage : ContentPage
    {
        public LoadingPage()
        {
            InitializeComponent();
            CheckLoginState();
        }

        private async void CheckLoginState()
        {
            try
            {
                var token = await SecureStorage.GetAsync("firebase_id_token");
                var email = await SecureStorage.GetAsync("user_email");

                System.Diagnostics.Debug.WriteLine($"[LoadingPage] Token: {token}");
                System.Diagnostics.Debug.WriteLine($"[LoadingPage] Email: {email}");

                if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(email))
                {
                    var firestore = new FirestoreService("seniordesigncapstoneproj-49cfd");
                    var (user, docId) = await firestore.GetUserByEmailAsync(email, token);

                    System.Diagnostics.Debug.WriteLine(user != null
                        ? $"[LoadingPage] User found: {user.Email}"
                        : "[LoadingPage] User not found in Firestore");

                    if (user != null && docId != null)
                    {
                        Application.Current.MainPage = new NavigationPage(new LandingPage(user));
                        return;
                    }
                }

                Application.Current.MainPage = new NavigationPage(new MainPage());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LoadingPage] Exception: {ex}");
                Application.Current.MainPage = new NavigationPage(new MainPage());
            }
        }
    }
}