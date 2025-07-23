using System.Text.Json;
using PolicyManagement.Models;

namespace PolicyManagement.Services;

public class JsonUserService : IUserService
{
    private readonly List<User>? _users;
    private readonly string _jsonPath;

    public JsonUserService()
    {
        _jsonPath = Path.Combine(AppContext.BaseDirectory, "Data/users.json");

        if (!File.Exists(_jsonPath))
            throw new FileNotFoundException($"User data file not found at: {_jsonPath}");

        var json = File.ReadAllText(_jsonPath);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        _users = JsonSerializer.Deserialize<List<User>>(json, options)
                 ?? new List<User>();
    }

    public Task<User?> AuthenticateAsync(string username, string password)
    {
        var user = _users?.FirstOrDefault(u =>
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
            && BCrypt.Net.BCrypt.Verify(password, u.PasswordHash)
        );

        return Task.FromResult(user);
    }

    public List<User> GetAllUsers()
    {
        return _users ?? new List<User>();
    }

    public void AddUser(User user)
    {
        if (_users == null) return;

        // ❗ Check if user exists
        if (_users.Any(u => u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase)))
            throw new Exception("User already exists");

        // ✅ Hash password
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

        _users.Add(user);

        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(_users, options);
        File.WriteAllText(_jsonPath, json);
    }
}