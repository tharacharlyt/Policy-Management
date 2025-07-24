using Microsoft.AspNetCore.Mvc;
using PolicyManagement.Models;
using PolicyManagement.Models.DTOs;
using PolicyManagement.DTOs;
using PolicyManagement.Services;
using PolicyManagement.Helpers;

namespace PolicyManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _config;

    private static readonly Dictionary<string, List<string>> RolePermissions = new()
    {
        ["Admin"] = new()
        {
            "Policy.Add", "Policy.Edit", "Policy.Delete", "Policy.List",
            "Document.Add", "Document.List"
        },
        ["Manager"] = new()
        {
            "Policy.Add", "Policy.Edit", "Policy.List", "Document.List"
        },
        ["Editor"] = new()
        {
            "Policy.Edit", "Document.List"
        },
        ["Viewer"] = new()
        {
            "Document.List"
        }
    };

    public AuthController(IUserService userService, IConfiguration config)
    {
        _userService = userService;
        _config = config;
    }

    // ------------------- REGISTER -------------------
    [HttpPost("register")]
    public IActionResult Register(RegisterRequest request)
    {
        var user = new User
        {
            UserId = Guid.NewGuid().ToString(),
            Name = request.Name,
            Username = request.Username,
            PasswordHash = request.Password, // Will be hashed in service
            Roles = request.Roles,
            Permissions = request.Roles
                .Where(r => RolePermissions.ContainsKey(r))
                .SelectMany(r => RolePermissions[r])
                .Distinct()
                .ToList(),
            RefreshToken = string.Empty,
            RefreshTokenExpiryTime = DateTime.MinValue
        };

        try
        {
            _userService.AddUser(user);
            return Ok(new { message = "✅ User registered successfully", username = user.Username });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // ------------------- LOGIN -------------------
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _userService.AuthenticateAsync(request.Username, request.Password);
        if (user == null)
            return Unauthorized("❌ Invalid username or password");

        var token = TokenHelper.GenerateJwtToken(user, _config);
        var refreshToken = TokenHelper.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        return Ok(new
        {
            token,
            refreshToken,
            username = user.Username
        });
    }

    // ------------------- REFRESH TOKEN -------------------
[HttpPost("refresh")]
public IActionResult RefreshToken(TokenRequest tokenRequest)
{
    var user = _userService.GetAllUsers()
        .FirstOrDefault(u => u.RefreshToken == tokenRequest.RefreshToken);

    if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
        return Unauthorized("Invalid or expired refresh token");

    var newToken = TokenHelper.GenerateJwtToken(user, _config);
    var newRefresh = TokenHelper.GenerateRefreshToken();

    user.RefreshToken = newRefresh;
    user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

    return Ok(new
    {
        token = newToken,
        refreshToken = newRefresh,
        username = user.Username
    });
}
    [HttpGet("throw")]
public IActionResult ThrowError()
{
    throw new Exception("This is a test exception!");
}
}