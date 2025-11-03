using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SeniorCapstoneProject.Models;
using System.Threading.Tasks;

namespace SeniorCapstoneProject.ViewModels
{
    public class CalendarPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Doctor _doctor;
        public Doctor Doctor
        {
            get => _doctor;
            set { _doctor = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Appointment> Appointments { get; set; } = new();

        private readonly FirestoreService _firestoreService = new FirestoreService("seniordesigncapstoneproj-49cfd");

        public CalendarPageViewModel(User user, string idToken)
        {
            LoadDoctorAndAppointments(user, idToken);
        }

        private async void LoadDoctorAndAppointments(User user, string idToken)
        {
            // Fetch doctor details using DoctorId
            if (!string.IsNullOrEmpty(user.DoctorId))
            {
                Doctor = await _firestoreService.GetDoctorByIdAsync(user.DoctorId, idToken);
            }

            // Fetch all appointments from the top-level collection, filter by userEmail
            var allAppointments = await _firestoreService.GetAppointmentsAsync(idToken);
            var userAppointments = allAppointments
                .Where(a => a.UserEmail == user.Email)
                .OrderBy(a => a.Date)
                .ToList();

            Appointments.Clear();
            foreach (var appt in userAppointments)
                Appointments.Add(appt);
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}