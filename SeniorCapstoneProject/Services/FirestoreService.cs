using SeniorCapstoneProject.ViewModels;
using SeniorCapstoneProject.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace SeniorCapstoneProject
{
    public class FirestoreService
    {
        private readonly string _projectId;
        private readonly HttpClient _httpClient;

        public FirestoreService(string projectId)
        {
            _projectId = projectId;
            _httpClient = new HttpClient();
        }

        public async Task<(User? user, string? docId)> GetUserByEmailAsync(string email, string idToken)
        {
            var url = $"https://firestore.googleapis.com/v1/projects/{_projectId}/databases/(default)/documents:runQuery";

            var query = new
            {
                structuredQuery = new
                {
                    from = new[] { new { collectionId = "users" } },
                    where = new
                    {
                        fieldFilter = new
                        {
                            field = new { fieldPath = "email" },
                            op = "EQUAL",
                            value = new { stringValue = email }
                        }
                    },
                    limit = 1
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonSerializer.Serialize(query), System.Text.Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return (null, null);

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);

            foreach (var result in doc.RootElement.EnumerateArray())
            {
                if (result.TryGetProperty("document", out var document))
                {
                    var fields = document.GetProperty("fields");
                    var name = document.GetProperty("name").GetString();
                    var docId = name?.Split('/').Last();

                    var user = new User
                    {
                        Email = fields.GetProperty("email").GetProperty("stringValue").GetString() ?? "",
                        FirstName = fields.GetProperty("firstName").GetProperty("stringValue").GetString() ?? "",
                        LastName = fields.GetProperty("lastName").GetProperty("stringValue").GetString() ?? "",
                        Username = fields.GetProperty("username").GetProperty("stringValue").GetString() ?? "",
                    };
                    return (user, docId);
                }
            }
            return (null, null);
        }

        public async Task<bool> SaveUserAsync(User user, string idToken)
        {
            var url = $"https://firestore.googleapis.com/v1/projects/{_projectId}/databases/(default)/documents/users?documentId={user.Username}";
            var doc = new
            {
                fields = new
                {
                    email = new { stringValue = user.Email },
                    firstName = new { stringValue = user.FirstName },
                    lastName = new { stringValue = user.LastName },
                    username = new { stringValue = user.Username }
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonSerializer.Serialize(doc), System.Text.Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> SaveAppointmentAsync(Appointment appointment, string idToken)
        {
            var url = $"https://firestore.googleapis.com/v1/projects/{_projectId}/databases/(default)/documents/appointments";
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

            var payload = new
            {
                fields = new
                {
                    DoctorName = new { stringValue = appointment.DoctorName },
                    Date = new { timestampValue = appointment.Date.ToString("o") },
                    TimeRange = new { stringValue = appointment.TimeRange }
                }
            };

            var response = await _httpClient.PostAsJsonAsync(url, payload);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<Appointment>> GetAppointmentsAsync(string idToken)
        {
            var url = $"https://firestore.googleapis.com/v1/projects/{_projectId}/databases/(default)/documents/appointments";
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return new List<Appointment>();

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);

            var appointments = new List<Appointment>();
            if (doc.RootElement.TryGetProperty("documents", out var docs))
            {
                foreach (var item in docs.EnumerateArray())
                {
                    var fields = item.GetProperty("fields");
                    var doctorName = fields.GetProperty("DoctorName").GetProperty("stringValue").GetString();
                    var dateStr = fields.GetProperty("Date").GetProperty("timestampValue").GetString();
                    var timeRange = fields.GetProperty("TimeRange").GetProperty("stringValue").GetString();

                    if (DateTime.TryParse(dateStr, out var date))
                    {
                        appointments.Add(new Appointment
                        {
                            DoctorName = doctorName,
                            Date = date,
                            TimeRange = timeRange
                        });
                    }
                }
            }
            return appointments;
        }

        public async Task<bool> SaveAppointmentForUserAsync(Appointment appointment, string userDocId, string idToken)
        {
            var url = $"https://firestore.googleapis.com/v1/projects/{_projectId}/databases/(default)/documents/users/{userDocId}/appointments";
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

            var payload = new
            {
                fields = new
                {
                    DoctorName = new { stringValue = appointment.DoctorName },
                    Date = new { timestampValue = appointment.Date.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'") },
                    TimeRange = new { stringValue = appointment.TimeRange }
                }
            };

            var response = await _httpClient.PostAsJsonAsync(url, payload);
            var responseContent = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"Firestore response: {responseContent}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<Appointment>> GetAppointmentsForUserAsync(string userDocId, string idToken)
        {
            var url = $"https://firestore.googleapis.com/v1/projects/{_projectId}/databases/(default)/documents/users/{userDocId}/appointments";
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

            var response = await _httpClient.GetAsync(url);
            var appointments = new List<Appointment>();

            if (!response.IsSuccessStatusCode)
                return appointments;

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("documents", out var docs))
            {
                foreach (var item in docs.EnumerateArray())
                {
                    var fields = item.GetProperty("fields");
                    var doctorName = fields.GetProperty("doctorName").GetProperty("stringValue").GetString();
                    var dateStr = fields.GetProperty("date").GetProperty("timestampValue").GetString();
                    var timeRange = fields.GetProperty("timeRange").GetProperty("stringValue").GetString();

                    if (DateTime.TryParse(dateStr, out var date))
                    {
                        appointments.Add(new Appointment
                        {
                            DoctorName = doctorName,
                            Date = date,
                            TimeRange = timeRange
                        });
                    }
                }
            }
            return appointments;
        }
    }
}