using SeniorCapstoneProject.ViewModels;

namespace SeniorCapstoneProject
{
    public partial class CalendarPage : ContentPage
    {
        private readonly string _userEmail;
        private readonly CalendarViewModel _viewModel;

        public CalendarPage(string userEmail)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            _userEmail = userEmail;
            _viewModel = new CalendarViewModel(_userEmail);
            BindingContext = _viewModel;

            if (_userEmail == "kaaslc@mail.uc.edu")
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