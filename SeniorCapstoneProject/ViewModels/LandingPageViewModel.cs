using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SeniorCapstoneProject.Models;
using System.Threading.Tasks;
using System.Linq;

namespace SeniorCapstoneProject.ViewModels
{
    public class LandingPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Appointment> Appointments { get; set; } = new();

        private readonly FirestoreService _firestoreService = new FirestoreService("seniordesigncapstoneproj-49cfd");
        private readonly User _user;
        private readonly string _idToken;

        public LandingPageViewModel(User user, string idToken)
        {
            _user = user;
            _idToken = idToken;
        }

        public async Task InitializeAsync()
        {
            var allAppointments = await _firestoreService.GetAppointmentsAsync(_idToken);
            var userAppointments = allAppointments
                .Where(a => a.UserEmail == _user.Email)
                .OrderBy(a => a.Date)
                .ToList();

            Appointments.Clear();
            foreach (var appt in userAppointments)
                Appointments.Add(appt);

            OnPropertyChanged(nameof(Appointments));
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}