using System.Net.Http;
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

        public async Task<User?> GetUserByEmailAsync(string email, string idToken)
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
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);

            foreach (var result in doc.RootElement.EnumerateArray())
            {
                if (result.TryGetProperty("document", out var document))
                {
                    var fields = document.GetProperty("fields");
                    return new User
                    {
                        Email = fields.GetProperty("email").GetProperty("stringValue").GetString() ?? "",
                        FirstName = fields.GetProperty("firstName").GetProperty("stringValue").GetString() ?? "",
                        LastName = fields.GetProperty("lastName").GetProperty("stringValue").GetString() ?? "",
                        Username = fields.GetProperty("username").GetProperty("stringValue").GetString() ?? "",
                    };
                }
            }
            return null;
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
    }
}