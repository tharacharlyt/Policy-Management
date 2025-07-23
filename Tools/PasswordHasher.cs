using System.Text.Json;
using PolicyManagement.Models;

namespace PolicyManagement.Tools;

public static class PasswordHasher
{
    public static void HashAllPasswords(string jsonFilePath)
    {
        if (!File.Exists(jsonFilePath))
        {
            Console.WriteLine("❌ File not found: " + jsonFilePath);
            return;
        }

        var json = File.ReadAllText(jsonFilePath);

        var users = JsonSerializer.Deserialize<List<User>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (users == null || users.Count == 0)
        {
            Console.WriteLine("❌ No users found or deserialization failed.");
            return;
        }

        foreach (var user in users)
        {
            if (!string.IsNullOrWhiteSpace(user.PasswordHash) && !user.PasswordHash.StartsWith("$2a$"))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
                Console.WriteLine($"✅ Hashed password for user: {user.Username}");
            }
        }

        var newJson = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(jsonFilePath, newJson);

        Console.WriteLine("\n🎉 All passwords hashed and saved successfully!");
    }
}