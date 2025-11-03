using SeniorCapstoneProject.Models;
using System.Net.Http.Json;
using System.Text.Json;

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

        #region Get User by Email

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
                        DoctorId = fields.TryGetProperty("DoctorId", out var doctorIdProp)
                            ? doctorIdProp.GetProperty("stringValue").GetString()
                            : null
                    };
                    return (user, docId);
                }
            }
            return (null, null);
        }

        #endregion Get User by Email

        #region Save New Patient

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
                    username = new { stringValue = user.Username },
                    DoctorId = new { stringValue = user.DoctorId ?? "" }
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

        #endregion Save New Patient

        #region Save Appointment

        //public async Task<bool> SaveAppointmentAsync(Appointment appointment, string idToken)
        //{
        //    var url = $"https://firestore.googleapis.com/v1/projects/{_projectId}/databases/(default)/documents/appointments";
        //    _httpClient.DefaultRequestHeaders.Authorization =
        //        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

        //    var payload = new
        //    {
        //        fields = new
        //        {
        //            DoctorName = new { stringValue = appointment.DoctorName },
        //            Date = new { timestampValue = appointment.Date.ToString("o") },
        //            TimeRange = new { stringValue = appointment.TimeRange },
        //            UserEmail = new { stringValue = appointment.UserEmail }
        //        }
        //    };

        //    var response = await _httpClient.PostAsJsonAsync(url, payload);
        //    var responseContent = await response.Content.ReadAsStringAsync();
        //    System.Diagnostics.Debug.WriteLine($"Firestore response: {response.StatusCode} - {responseContent}");

        //    return response.IsSuccessStatusCode;
        //}

        #endregion Save Appointment

        #region Get Appointments 

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
                    var doctorId = fields.GetProperty("doctorId").GetProperty("stringValue").GetString();
                    var doctorName = fields.GetProperty("doctorName").GetProperty("stringValue").GetString();
                    var dateStr = fields.GetProperty("date").GetProperty("timestampValue").GetString();
                    var timeRange = fields.GetProperty("timeRange").GetProperty("stringValue").GetString();
                    var userEmail = fields.GetProperty("userEmail").GetProperty("stringValue").GetString();

                    if (DateTime.TryParse(dateStr, out var date))
                    {
                        appointments.Add(new Appointment
                        {
                            DoctorId = doctorId,
                            DoctorName = doctorName,
                            Date = date,
                            TimeRange = timeRange,
                            UserEmail = userEmail
                        });
                    }
                }
            }
            return appointments;
        }

        #endregion Get Appointments

        #region Save Appointment for User

        public async Task<bool> SaveAppointmentForUserAsync(Appointment appointment, string userDocId, string idToken)
        {
            var url = $"https://firestore.googleapis.com/v1/projects/{_projectId}/databases/(default)/documents/users/{userDocId}/appointments";
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

            var payload = new
            {
                fields = new
                {
                    doctorId = new { stringValue = appointment.DoctorId },
                    doctorName = new { stringValue = appointment.DoctorName },
                    date = new { timestampValue = appointment.Date.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'") },
                    timeRange = new { stringValue = appointment.TimeRange },
                    userEmail = new { stringValue = appointment.UserEmail }
                }
            };

            var response = await _httpClient.PostAsJsonAsync(url, payload);
            var responseContent = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"Firestore response: {responseContent}");
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
                    doctorId = new { stringValue = appointment.DoctorId },
                    doctorName = new { stringValue = appointment.DoctorName },
                    date = new { timestampValue = appointment.Date.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'") },
                    timeRange = new { stringValue = appointment.TimeRange },
                    userEmail = new { stringValue = appointment.UserEmail }
                }
            };

            var response = await _httpClient.PostAsJsonAsync(url, payload);
            var responseContent = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"Firestore response: {response.StatusCode} - {responseContent}");

            return response.IsSuccessStatusCode;
        }

        #endregion Save Appointment for User

        #region Get Appointments for User

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

        #endregion Get Appointments for User

        #region Medication Methods

        #region Get Medications

        public async Task<List<Medication>> GetMedicationsForUserAsync(string userEmail, string idToken)
        {
            var url = $"https://firestore.googleapis.com/v1/projects/{_projectId}/databases/(default)/documents/medications";
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

            var response = await _httpClient.GetAsync(url);
            var medications = new List<Medication>();

            if (!response.IsSuccessStatusCode)
                return medications;

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("documents", out var docs))
            {
                foreach (var item in docs.EnumerateArray())
                {
                    var fields = item.GetProperty("fields");
                    var email = fields.GetProperty("UserEmail").GetProperty("stringValue").GetString();
                    if (email != userEmail) continue;

                    medications.Add(new Medication
                    {
                        Name = fields.GetProperty("Name").GetProperty("stringValue").GetString(),
                        Dosage = fields.GetProperty("Dosage").GetProperty("stringValue").GetString(),
                        Instructions = fields.GetProperty("Instructions").GetProperty("stringValue").GetString(),
                        UserEmail = email
                    });
                }
            }
            return medications;
        }

        #endregion Get Medications

        #region Save Medication

        public async Task<bool> SaveMedicationAsync(Medication medication, string idToken)
        {
            var url = $"https://firestore.googleapis.com/v1/projects/{_projectId}/databases/(default)/documents/medications";
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

            var payload = new
            {
                fields = new
                {
                    Name = new { stringValue = medication.Name },
                    Dosage = new { stringValue = medication.Dosage },
                    Instructions = new { stringValue = medication.Instructions },
                    UserEmail = new { stringValue = medication.UserEmail }
                }
            };

            var response = await _httpClient.PostAsJsonAsync(url, payload);
            return response.IsSuccessStatusCode;
        }

        #endregion Save Medication

        #endregion Medication Methods

        #region Get Doctors

        public async Task<Doctor?> GetDoctorByIdAsync(string doctorId, string idToken)
        {
            var url = $"https://firestore.googleapis.com/v1/projects/{_projectId}/databases/(default)/documents/doctors/{doctorId}";
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("fields", out var fields))
            {
                return new Doctor
                {
                    Id = doctorId,
                    Name = fields.GetProperty("name").GetProperty("stringValue").GetString() ?? "",
                    Specialty = fields.GetProperty("specialty").GetProperty("stringValue").GetString() ?? "",
                    Description = fields.GetProperty("description").GetProperty("stringValue").GetString() ?? "",
                    ImagePath = fields.GetProperty("imagePath").GetProperty("stringValue").GetString() ?? ""
                };
            }
            return null;
        }

        public async Task<Doctor?> GetDoctorByNameAsync(string doctorName, string idToken)
        {
            var url = $"https://firestore.googleapis.com/v1/projects/{_projectId}/databases/(default)/documents:runQuery";
            var query = new
            {
                structuredQuery = new
                {
                    from = new[] { new { collectionId = "doctors" } },
                    where = new
                    {
                        fieldFilter = new
                        {
                            field = new { fieldPath = "name" },
                            op = "EQUAL",
                            value = new { stringValue = doctorName }
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
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);

            foreach (var result in doc.RootElement.EnumerateArray())
            {
                if (result.TryGetProperty("document", out var document))
                {
                    var fields = document.GetProperty("fields");
                    return new Doctor
                    {
                        Name = fields.GetProperty("name").GetProperty("stringValue").GetString() ?? "",
                        Specialty = fields.GetProperty("specialty").GetProperty("stringValue").GetString() ?? "",
                        Description = fields.GetProperty("description").GetProperty("stringValue").GetString() ?? "",
                        ImagePath = fields.GetProperty("imagePath").GetProperty("stringValue").GetString() ?? ""
                    };
                }
            }
            return null;
        }

        #endregion Get Doctors
    }
}