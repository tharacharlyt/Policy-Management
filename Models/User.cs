namespace PolicyManagement.Models;

public class User
{
    public string UserId { get; set; } = Guid.NewGuid().ToString();  // Auto-generate if not passed
    public string Name { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty; // Use this, not plain Password
    public List<string> Roles { get; set; } = new();
    public List<string> Permissions { get; set; } = new();
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiryTime { get; set; }
}