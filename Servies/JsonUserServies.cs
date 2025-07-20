using System.Text.Json;
using PolicyManagement.Models;

namespace PolicyManagement.Services
{
    public interface IUserService
    {
        Task<User?> AuthenticateAsync(string username, string password);
    }

    public class JsonUserService : IUserService
    {
        private readonly List<User> _users;

        public JsonUserService()
        {
            var jsonPath = Path.Combine(AppContext.BaseDirectory, "Data/users.json");
            if (!File.Exists(jsonPath))
                throw new FileNotFoundException($"User data file not found at: {jsonPath}");

            var json = File.ReadAllText(jsonPath);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            _users = JsonSerializer.Deserialize<List<User>>(json, options)
                     ?? throw new InvalidOperationException("Failed to load user data from users.json");
        }

        public Task<User?> AuthenticateAsync(string username, string password)
        {
            var user = _users.FirstOrDefault(u =>
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
                u.Password == password
            );
            return Task.FromResult(user);
        }
    }
}