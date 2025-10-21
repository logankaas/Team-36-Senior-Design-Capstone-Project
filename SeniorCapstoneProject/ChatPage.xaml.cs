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
            Messages.Add(new ChatMessage { Text = botResponse, IsUser = false });

            IsBotLoading = false;
            OnPropertyChanged(nameof(IsBotLoading));
        }

        private async Task<string> GetBotResponseAsync(string userText)
        {
            userText = userText.ToLower();

            // Doctor selection
            if (userText.Contains("schedule") || userText.Contains("appointment"))
            {
                _pendingDoctor = "Dr. Markins"; // You can prompt for doctor selection or use a default
                _pendingDate = DateTime.Today.AddDays(1);

                // Show available times
                var availableTimes = new List<string> { "9:00 AM", "10:00 AM", "2:00 PM", "3:30 PM" };
                Messages.Add(new ChatMessage
                {
                    Text = $"Select an available time for {_pendingDoctor} on {_pendingDate:MMM dd}:",
                    IsUser = false,
                    AvailableTimes = availableTimes,
                    DoctorName = _pendingDoctor,
                    IsTimeSelection = true
                });
                return null; // No bot text, handled by message above
            }

            // Confirmation after time selection
            if (!string.IsNullOrEmpty(_pendingTime))
            {
                // Confirm and save appointment
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
                _pendingTime = null; // Reset
                return success
                    ? $"Your appointment with {_pendingDoctor} on {_pendingDate:MMM dd} at {_pendingTime} is confirmed and saved. See you then!"
                    : "Sorry, there was a problem saving your appointment. Please try again.";
            }

            // Fun responses
            if (userText.Contains("joke"))
                return "Why did the doctor carry a red pen? In case they needed to draw blood! 😄";

            if (userText.Contains("hello") || userText.Contains("hi"))
                return $"Hello {_user.FirstName ?? "there"}! How can I help you today?";

            if (userText.Contains("schedule") || userText.Contains("appointment"))
                return "To schedule an appointment, please tell me your preferred doctor and date. Or type 'next available' for the soonest slot.";

            if (userText.Contains("upcoming") || userText.Contains("next appointment"))
                return "Your next appointment is with Dr. Markins on June 4th at 15:00. Would you like to reschedule or get directions?";

            if (userText.Contains("past") || userText.Contains("previous appointment"))
                return "Your last appointment was with Dr. Markins on June 1st at 10:00. Need a summary or follow-up?";

            if (userText.Contains("medicine") || userText.Contains("prescription"))
                return "You have a prescription for Amoxicillin. Would you like to request a refill or see details?";

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

        public void OnTimeSelected(string selectedTime)
        {
            _pendingTime = selectedTime;
            Messages.Add(new ChatMessage
            {
                Text = $"You selected {_pendingTime}. Confirming...",
                IsUser = true
            });

            // Trigger bot confirmation and save
            Device.BeginInvokeOnMainThread(async () =>
            {
                IsBotLoading = true;
                OnPropertyChanged(nameof(IsBotLoading));
                await Task.Delay(1000);
                var botResponse = await GetBotResponseAsync(""); // Trigger the confirmation
                if (!string.IsNullOrEmpty(botResponse))
                    Messages.Add(new ChatMessage { Text = botResponse, IsUser = false });
                IsBotLoading = false;
                OnPropertyChanged(nameof(IsBotLoading));
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