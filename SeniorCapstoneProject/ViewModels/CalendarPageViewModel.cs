using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SeniorCapstoneProject.Models;
using System.Threading.Tasks;
using System.Linq;

namespace SeniorCapstoneProject.ViewModels
{
    public class CalendarPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Appointment> Appointments { get; set; } = new();
        public ObservableCollection<Appointment> PastAppointments { get; set; } = new();

        private readonly FirestoreService _firestoreService = new FirestoreService("seniordesigncapstoneproj-49cfd");
        private readonly User _user;
        private readonly string _idToken;

        private string _currentMonth;
        public string CurrentMonth
        {
            get => _currentMonth;
            set { _currentMonth = value; OnPropertyChanged(); }
        }

        private int _appointmentCount;
        public int AppointmentCount
        {
            get => _appointmentCount;
            set { _appointmentCount = value; OnPropertyChanged(); }
        }

        private string _nextAppointmentText;
        public string NextAppointmentText
        {
            get => _nextAppointmentText;
            set { _nextAppointmentText = value; OnPropertyChanged(); }
        }

        private bool _hasPastAppointments;
        public bool HasPastAppointments
        {
            get => _hasPastAppointments;
            set { _hasPastAppointments = value; OnPropertyChanged(); }
        }

        private int _totalAppointments;
        public int TotalAppointments
        {
            get => _totalAppointments;
            set { _totalAppointments = value; OnPropertyChanged(); }
        }

        private string _hoursSpent;
        public string HoursSpent
        {
            get => _hoursSpent;
            set { _hoursSpent = value; OnPropertyChanged(); }
        }

        public CalendarPageViewModel(User user, string idToken)
        {
            _user = user;
            _idToken = idToken;
            CurrentMonth = DateTime.Now.ToString("MMMM yyyy");
        }

        public async Task LoadAppointmentsAsync()
        {
            var allAppointments = await _firestoreService.GetAppointmentsAsync(_idToken);
            var userAppointments = allAppointments
                .Where(a => a.UserEmail == _user.Email)
                .OrderBy(a => a.Date)
                .ToList();

            var now = DateTime.Now;
            var upcomingAppts = userAppointments.Where(a => a.Date >= now).ToList();
            var pastAppts = userAppointments.Where(a => a.Date < now).OrderByDescending(a => a.Date).ToList();

            Appointments.Clear();
            foreach (var appt in upcomingAppts)
                Appointments.Add(appt);

            PastAppointments.Clear();
            foreach (var appt in pastAppts.Take(5)) // Show only last 5
                PastAppointments.Add(appt);

            AppointmentCount = Appointments.Count;
            HasPastAppointments = PastAppointments.Any();

            if (Appointments.Any())
            {
                var nextAppt = Appointments.First();
                var daysUntil = (nextAppt.Date - now).Days;
                NextAppointmentText = daysUntil == 0
                    ? "Next appointment is today!"
                    : $"Next appointment in {daysUntil} day{(daysUntil > 1 ? "s" : "")}";
            }
            else
            {
                NextAppointmentText = "No upcoming appointments";
            }

            // Stats
            TotalAppointments = userAppointments.Count(a => a.Date.Month == now.Month && a.Date.Year == now.Year);
            HoursSpent = (TotalAppointments * 1.5).ToString("F1"); // Estimate 1.5 hours per appointment

            OnPropertyChanged(nameof(Appointments));
            OnPropertyChanged(nameof(PastAppointments));
        }

        public async Task DeleteAppointmentAsync(Appointment appointment)
        {
            //await _firestoreService.DeleteAppointmentAsync(appointment.Id, _idToken);
            Appointments.Remove(appointment);
            AppointmentCount = Appointments.Count;
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}