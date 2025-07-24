// Models/User.cs
namespace PolicyManagement.Models;

public class User
{
    public string UserId { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "";
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public List<string> Roles { get; set; } = new();
    public List<string> Permissions { get; set; } = new();
    public string RefreshToken { get; set; } = "";
    public DateTime RefreshTokenExpiryTime { get; set; }
}