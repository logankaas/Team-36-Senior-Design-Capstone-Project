using SeniorCapstoneProject.ViewModels;

namespace SeniorCapstoneProject
{
    public partial class MedicationsPage : ContentPage
    {
        private MedicationsViewModel _viewModel;

        public MedicationsPage(string userEmail, string idToken)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            _viewModel = new MedicationsViewModel(userEmail, idToken);
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Load medications when page appears
            _viewModel.LoadMedicationsCommand.Execute(null);
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}