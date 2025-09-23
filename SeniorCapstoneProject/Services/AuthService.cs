using System.Data.SqlClient;

public class AuthService
{
    private readonly string _connectionString;

    public AuthService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public bool Login(string username, string password)
    {
        const string query = "SELECT COUNT(1) FROM users WHERE username = @username AND password_hash = @password";

        using (var connection = new SqlConnection(_connectionString))
        using (var command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@password", password); // hashed password

            connection.Open();
            int count = (int)command.ExecuteScalar();
            return count == 1;
        }
    }
}