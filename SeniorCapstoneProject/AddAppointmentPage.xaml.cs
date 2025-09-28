using SeniorCapstoneProject.Models;
using SeniorCapstoneProject.ViewModels;

namespace SeniorCapstoneProject
{
    public partial class AddAppointmentPage : ContentPage
    {
        public AddAppointmentPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void OnAddAppointmentClicked(object sender, EventArgs e)
        {
            try
            {
                var selectedUser = UserPicker.SelectedItem as User;
                if (selectedUser == null)
                {
                    await DisplayAlert("Error", "Please select a user.", "OK");
                    return;
                }

                var selectedDoctor = DoctorPicker.SelectedItem as Doctor;
                if (selectedDoctor == null)
                {
                    await DisplayAlert("Error", "Please select a doctor.", "OK");
                    return;
                }

                var selectedTime = TimePicker.SelectedItem as string;
                if (string.IsNullOrEmpty(selectedTime))
                {
                    await DisplayAlert("Error", "Please select a time slot.", "OK");
                    return;
                }

                var selectedDate = DatePicker.Date;

                var idToken = await SecureStorage.GetAsync("firebase_id_token");
                var firestoreService = new FirestoreService("seniordesigncapstoneproj-49cfd");

                var userDocId = selectedUser.Id;
                var userEmail = selectedUser.Email;

                var appointment = new Appointment
                {
                    DoctorName = selectedDoctor.Name,
                    Date = selectedDate,
                    TimeRange = selectedTime,
                    UserEmail = userEmail
                };

                var success = await firestoreService.SaveAppointmentForUserAsync(appointment, userDocId.ToString(), idToken);

                if (success)
                {
                    await DisplayAlert("Success", "Appointment added.", "OK");
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Error", "Failed to add appointment.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }
    }
}