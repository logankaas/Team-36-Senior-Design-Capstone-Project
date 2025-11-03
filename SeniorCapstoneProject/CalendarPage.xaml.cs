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

            if (_user.Email == "kaaslc@mail.uc.edu")
            {
                var addAppointmentToolbar = new ToolbarItem
                {
                    Text = "＋",
                    Order = ToolbarItemOrder.Primary,
                    Priority = 0
                };
                addAppointmentToolbar.Clicked += OnAddAppointmentClicked;
                ToolbarItems.Add(addAppointmentToolbar);
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.InitializeAsync();
        }

        private async void OnAddAppointmentClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddAppointmentPage());
        }
    }
}