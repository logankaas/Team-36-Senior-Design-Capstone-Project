using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using SeniorCapstoneProject.Models;

namespace SeniorCapstoneProject.ViewModels
{
    public class CalendarViewModel
    {
        public ObservableCollection<Appointment> Appointments { get; set; }
        private readonly FirestoreService _firestoreService;
        private string _idToken;
        private string _userEmail;

        public CalendarViewModel(string userEmail)
        {
            Appointments = new ObservableCollection<Appointment>();
            _firestoreService = new FirestoreService("seniordesigncapstoneproj-49cfd");
            _userEmail = userEmail;
        }

        public async Task InitializeAsync()
        {
            try
            {
                _idToken = await SecureStorage.GetAsync("firebase_id_token");
                var (user, userDocId) = await _firestoreService.GetUserByEmailAsync(_userEmail, _idToken);
                if (user != null && userDocId != null)
                {
                    var appointments = await _firestoreService.GetAppointmentsForUserAsync(userDocId, _idToken);
                    Appointments.Clear();
                    foreach (var appt in appointments)
                    {
                        Appointments.Add(appt);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CalendarViewModel] Exception: {ex}");
            }
        }
    }
}