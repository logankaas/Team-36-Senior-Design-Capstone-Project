using SQLite;
using System.Threading.Tasks;
using System.IO;

public class UserDatabase
{
    private readonly SQLiteAsyncConnection _db;

    public UserDatabase(string dbPath)
    {
        _db = new SQLiteAsyncConnection(dbPath);
        _db.CreateTableAsync<User>().Wait();
    }

    public Task<User> GetUserAsync(string username, string password) =>
        _db.Table<User>().FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

    public Task<User> GetUserByUsernameAsync(string username) =>
        _db.Table<User>().FirstOrDefaultAsync(u => u.Username == username);

    public Task<int> AddUserAsync(User user) =>
        _db.InsertAsync(user);

    public Task<int> UpdateUserAsync(User user) =>
        _db.UpdateAsync(user);
}