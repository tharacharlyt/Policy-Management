// Models/DTOs/RegisterDto.cs
using System.ComponentModel.DataAnnotations;

namespace PolicyManagement.Models.DTOs;

public class RegisterDto
{
    [Required]
    public string Name { get; set; } = "";

    [Required]
    public string Username { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";

    [Required]
    public string Role { get; set; } = "";  // Admin, Manager, etc.
}