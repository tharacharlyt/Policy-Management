using System.Text.Json;
using PolicyManagement.Models;

namespace PolicyManagement.Services
{
    public class JsonUserService : IUserService
    {
        private readonly string _jsonPath;
        private readonly List<User> _users;

        public JsonUserService()
        {
            // ✅ This ensures the path is correct: /YourProjectRoot/Data/users.json
            _jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "users.json");

            if (!File.Exists(_jsonPath))
                throw new FileNotFoundException($"❌ User data file not found at: {_jsonPath}");

            var json = File.ReadAllText(_jsonPath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            _users = JsonSerializer.Deserialize<List<User>>(json, options) ?? new List<User>();
        }

        public Task<User?> AuthenticateAsync(string username, string password)
        {
            var user = _users.FirstOrDefault(u =>
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
                BCrypt.Net.BCrypt.Verify(password, u.PasswordHash));

            return Task.FromResult(user);
        }

        public List<User> GetAllUsers() => _users;

        public void AddUser(User user)
        {
            if (_users.Any(u => u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase)))
                throw new Exception("User already exists");

            user.UserId = Guid.NewGuid().ToString();
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            user.RefreshToken = string.Empty;
            user.RefreshTokenExpiryTime = DateTime.MinValue;

            _users.Add(user);

            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(_users, options);
            File.WriteAllText(_jsonPath, json);

            Console.WriteLine($"✅ User '{user.Username}' saved to JSON at: {_jsonPath}");
        }
    }
}