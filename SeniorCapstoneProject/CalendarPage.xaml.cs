using SeniorCapstoneProject.ViewModels;
using SeniorCapstoneProject.Models;

namespace SeniorCapstoneProject
{
    public partial class CalendarPage : ContentPage
    {
        private readonly User _user;
        private readonly CalendarViewModel _viewModel;

        public CalendarPage(User user, string idToken)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            _user = user;
            _viewModel = new CalendarViewModel(_user, idToken);
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.InitializeAsync();
        }

        private void OnBackButtonClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}