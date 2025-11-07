using SeniorCapstoneProject.ViewModels;

namespace SeniorCapstoneProject
{
    public partial class MedicationsPage : ContentPage
    {
        public MedicationsPage(string userEmail, string idToken)
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

            var vm = new MedicationsViewModel(userEmail, idToken);
            BindingContext = vm;
            vm.LoadMedicationsCommand.Execute(null);
        }

        private void OnBackButtonClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}