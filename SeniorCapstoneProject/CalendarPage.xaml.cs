using SeniorCapstoneProject.ViewModels;
using SeniorCapstoneProject.Models;

namespace SeniorCapstoneProject
{
    public partial class CalendarPage : ContentPage
    {
        private readonly User _user;
        private readonly string _idToken;
        private readonly CalendarPageViewModel _viewModel;

        public CalendarPage(User user, string idToken)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            _user = user;
            _idToken = idToken;
            _viewModel = new CalendarPageViewModel(_user, idToken);
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadAppointmentsAsync();
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnAddAppointmentClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddAppointmentPage());
        }

        private async void OnEditAppointmentClicked(object sender, TappedEventArgs e)
        {
            if (e.Parameter is Appointment appointment)
            {
                // TODO: Navigate to edit appointment page when created
                await DisplayAlert("Edit", $"Edit appointment with {appointment.DoctorName}", "OK");
            }
        }

        private async void OnDeleteAppointmentClicked(object sender, TappedEventArgs e)
        {
            if (e.Parameter is Appointment appointment)
            {
                bool confirm = await DisplayAlert(
                    "Delete Appointment",
                    $"Are you sure you want to delete your appointment with {appointment.DoctorName}?",
                    "Delete",
                    "Cancel");

                if (confirm)
                {
                    await _viewModel.DeleteAppointmentAsync(appointment);
                    await DisplayAlert("Deleted", "Appointment has been deleted", "OK");
                }
            }
        }
    }
}