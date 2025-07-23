using System.ComponentModel.DataAnnotations;

namespace PolicyManagement.Models.DTOs;

public class RolePermissionRequest
{
    [Required]
    public required string Username { get; set; }

    [Required]
    [MinLength(1)]
    public required List<string> Roles { get; set; }

    [Required]
    [MinLength(1)]
    public required List<string> Permissions { get; set; }
}