// Services/JsonUserService.cs
using System.Text.Json;
using PolicyManagement.Models;

namespace PolicyManagement.Services;

public class JsonUserService : IUserService
{
    private readonly string _jsonPath;
    private readonly List<User> _users;

    private static readonly Dictionary<string, List<string>> RolePermissions = new()
    {
        ["Admin"] = new() {
            "Policy.Add", "Policy.Edit", "Policy.Delete", "Policy.List",
            "Document.Add", "Document.List"
        },
        ["Manager"] = new() {
            "Policy.Add", "Policy.Edit", "Policy.List", "Document.List"
        },
        ["Editor"] = new() {
            "Policy.Edit", "Document.List"
        },
        ["Viewer"] = new() {
            "Document.List"
        }
    };

    public JsonUserService()
    {
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

        if (!RolePermissions.ContainsKey(user.Roles.First()))
            throw new Exception("Invalid role");

        user.UserId = Guid.NewGuid().ToString();
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
        user.RefreshToken = string.Empty;
        user.RefreshTokenExpiryTime = DateTime.MinValue;

        user.Permissions = RolePermissions[user.Roles.First()];

        _users.Add(user);

        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(_users, options);
        File.WriteAllText(_jsonPath, json);

        Console.WriteLine($"✅ User '{user.Username}' saved to JSON at: {_jsonPath}");
    }
}