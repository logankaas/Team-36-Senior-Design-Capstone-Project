using SeniorCapstoneProject.Models;

namespace SeniorCapstoneProject
{
    public partial class MedicalIdPage : ContentPage
    {
        public MedicalIdPage(User user)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            BindingContext = user;
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}