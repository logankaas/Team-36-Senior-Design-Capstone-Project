using Microsoft.Maui.Controls;
using SeniorCapstoneProject.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SeniorCapstoneProject
{
    public partial class ChatPage : ContentPage
    {
        public ObservableCollection<ChatMessage> Messages { get; set; } = new();
        public ICommand SelectTimeCommand { get; }
        public bool IsBotLoading { get; set; }

        private readonly User _user;
        private string _pendingDoctor;
        private DateTime _pendingDate;
        private string _pendingTime;

        private enum ChatContext { None, Appointment, Medication }
        private ChatContext _currentContext = ChatContext.None;
        private string _selectedMedication = null;

        public ChatPage(User user)
        {
            InitializeComponent();
            BindingContext = this;
            _user = user;
            SelectTimeCommand = new Command<string>(OnTimeSelected);

            Messages.CollectionChanged += (s, e) => ScrollToBottom();

            Messages.Add(new ChatMessage
            {
                Text = "👋 Hi! I'm your health assistant. Ask me about appointments, prescriptions, tests, or just say hello!",
                IsUser = false
            });
        }

        private async void OnSendClicked(object sender, EventArgs e)
        {
            var userText = MessageEntry.Text?.Trim();
            if (string.IsNullOrEmpty(userText))
                return;

            Messages.Add(new ChatMessage { Text = userText, IsUser = true });
            MessageEntry.Text = string.Empty;

            IsBotLoading = true;
            OnPropertyChanged(nameof(IsBotLoading));

            await Task.Delay(1200); // Simulate bot thinking

            var botResponse = await GetBotResponseAsync(userText);
            if (!string.IsNullOrEmpty(botResponse))
                Messages.Add(new ChatMessage { Text = botResponse, IsUser = false });

            IsBotLoading = false;
            OnPropertyChanged(nameof(IsBotLoading));
        }

        private async Task<string> GetBotResponseAsync(string userText)
        {
            userText = userText.ToLower();

            // Appointment flow
            if (userText.Contains("schedule") || userText.Contains("appointment"))
            {
                _currentContext = ChatContext.Appointment;
                _pendingDoctor = "Dr. Markins";
                _pendingDate = DateTime.Today.AddDays(1);

                var availableTimes = new List<string> { "9:00 AM", "10:00 AM", "2:00 PM", "3:30 PM" };
                Messages.Add(new ChatMessage
                {
                    Text = $"Select an available time for {_pendingDoctor} on {_pendingDate:MMM dd}:",
                    IsUser = false,
                    AvailableTimes = availableTimes,
                    DoctorName = _pendingDoctor,
                    IsTimeSelection = true
                });
                return null;
            }

            // Appointment time confirmation
            if (_currentContext == ChatContext.Appointment && !string.IsNullOrEmpty(_pendingTime))
            {
                var idToken = await SecureStorage.GetAsync("firebase_id_token");
                var firestoreService = new FirestoreService("seniordesigncapstoneproj-49cfd");
                var appointment = new Appointment
                {
                    DoctorName = _pendingDoctor,
                    Date = _pendingDate,
                    TimeRange = _pendingTime,
                    UserEmail = _user.Email
                };
                var success = await firestoreService.SaveAppointmentAsync(appointment, idToken);
                _pendingTime = null;
                _currentContext = ChatContext.None;
                return success
                    ? $"Your appointment with {_pendingDoctor} on {_pendingDate:MMM dd} at {appointment.TimeRange} is confirmed and saved. See you then!"
                    : "Sorry, there was a problem saving your appointment. Please try again.";
            }

            // Medication flow
            if (userText.Contains("medicine") || userText.Contains("prescription"))
            {
                _currentContext = ChatContext.Medication;
                var idToken = await SecureStorage.GetAsync("firebase_id_token");
                var firestoreService = new FirestoreService("seniordesigncapstoneproj-49cfd");
                var medications = await firestoreService.GetMedicationsForUserAsync(_user.Email, idToken);

                if (medications.Count == 0)
                    return "You have no medications on file.";

                var medNames = medications.Select(m => m.Name).ToList();
                Messages.Add(new ChatMessage
                {
                    Text = "Here are your current prescriptions. Tap one to request a refill or see details:",
                    IsUser = false,
                    AvailableTimes = medNames,
                    IsTimeSelection = true
                });

                return null;
            }

            // Medication selection confirmation
            if (_currentContext == ChatContext.Medication && !string.IsNullOrEmpty(_selectedMedication))
            {
                var medName = _selectedMedication;
                _selectedMedication = null;
                _currentContext = ChatContext.None;
                return $"Refill request for {medName} received. Your provider will review and notify you when it's ready.";
            }

            // Fun responses
            if (userText.Contains("joke"))
                return "Why did the doctor carry a red pen? In case they needed to draw blood! 😄";

            if (userText.Contains("hello") || userText.Contains("hi"))
                return $"Hello {_user.FirstName ?? "there"}! How can I help you today?";

            if (userText.Contains("upcoming") || userText.Contains("next appointment"))
                return "Your next appointment is with Dr. Markins on June 4th at 15:00. Would you like to reschedule or get directions?";

            if (userText.Contains("past") || userText.Contains("previous appointment"))
                return "Your last appointment was with Dr. Markins on June 1st at 10:00. Need a summary or follow-up?";

            if (userText.Contains("test") || userText.Contains("lab"))
                return "Your last lab test was a blood panel on May 28th. Results are normal. Want to schedule another test?";

            if (userText.Contains("help"))
                return "You can ask me about appointments, prescriptions, tests, or just chat! Try: 'Schedule appointment', 'Show my prescriptions', or 'Tell me a joke'.";

            if (userText.Contains("thank"))
                return "You're welcome! 😊";

            // fuzzy matching for keywords
            var keywords = new[] { "appointment", "prescription", "test", "doctor", "medicine", "lab" };
            if (keywords.Any(k => userText.Contains(k)))
                return "I can help with appointments, prescriptions, and tests. Please be more specific!";

            // Default fallback
            return "I didn't understand that but I'm here to help! You can ask me about your health, appointments, or just say hi.";
        }

        public void OnTimeSelected(string selectedOption)
        {
            if (_currentContext == ChatContext.Medication)
            {
                _selectedMedication = selectedOption;
                Messages.Add(new ChatMessage
                {
                    Text = $"Refill request for {_selectedMedication} is being processed...",
                    IsUser = true
                });

                Device.BeginInvokeOnMainThread(async () =>
                {
                    IsBotLoading = true;
                    OnPropertyChanged(nameof(IsBotLoading));
                    await Task.Delay(1000);
                    var botResponse = await GetBotResponseAsync(""); // Triggers medication confirmation
                    if (!string.IsNullOrEmpty(botResponse))
                        Messages.Add(new ChatMessage { Text = botResponse, IsUser = false });
                    IsBotLoading = false;
                    OnPropertyChanged(nameof(IsBotLoading));
                });
                return;
            }

            if (_currentContext == ChatContext.Appointment)
            {
                _pendingTime = selectedOption;
                Messages.Add(new ChatMessage
                {
                    Text = $"You selected {_pendingTime}. Confirming...",
                    IsUser = true
                });

                Device.BeginInvokeOnMainThread(async () =>
                {
                    IsBotLoading = true;
                    OnPropertyChanged(nameof(IsBotLoading));
                    await Task.Delay(1000);
                    var botResponse = await GetBotResponseAsync(""); // Triggers appointment confirmation
                    if (!string.IsNullOrEmpty(botResponse))
                        Messages.Add(new ChatMessage { Text = botResponse, IsUser = false });
                    IsBotLoading = false;
                    OnPropertyChanged(nameof(IsBotLoading));
                });
                return;
            }

            // Fallback if context not set
            Messages.Add(new ChatMessage
            {
                Text = $"You selected {selectedOption}.",
                IsUser = false
            });
        }

        private void ScrollToBottom()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (Messages.Count > 0)
                    ChatCollectionView.ScrollTo(Messages.Last(), position: ScrollToPosition.End, animate: true);
            });
        }
    }
}