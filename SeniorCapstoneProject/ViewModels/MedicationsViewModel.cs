using SeniorCapstoneProject.Models;
using System.Collections.ObjectModel;

namespace SeniorCapstoneProject.ViewModels
{
    public class MedicationsViewModel : BaseViewModel
    {
        public ObservableCollection<Medication> Medications { get; } = new();
        private readonly FirestoreService _firestoreService;
        private readonly string _userEmail;
        private readonly string _idToken;

        public MedicationsViewModel(string userEmail, string idToken)
        {
            _userEmail = userEmail;
            _idToken = idToken;
            _firestoreService = new FirestoreService("seniordesigncapstoneproj-49cfd");
            LoadMedicationsCommand = new Command(async () => await LoadMedicationsAsync());
        }

        public Command LoadMedicationsCommand { get; }

        private async Task LoadMedicationsAsync()
        {
            IsBusy = true;
            Medications.Clear();

            var meds = await _firestoreService.GetMedicationsByUserEmailAsync(_userEmail, _idToken);

            foreach (var med in meds)
                Medications.Add(med);

            IsBusy = false;
        }
    }
}