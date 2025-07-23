using Microsoft.AspNetCore.Mvc;
using PolicyManagement.Models;
using PolicyManagement.Models.DTOs;
using PolicyManagement.Services;
using PolicyManagement.Helpers;

namespace PolicyManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _config;

    public AuthController(IUserService userService, IConfiguration config)
    {
        _userService = userService;
        _config = config;
    }

    [HttpPost("register")]
    public IActionResult Register(RegisterRequest request)
    {
        var user = new User
        {
            Name = request.Name,
            Username = request.Username,
            PasswordHash = request.Password, // Will be hashed in service
            Roles = request.Roles,
            Permissions = request.Permissions
        };

        try
        {
            _userService.AddUser(user);
            return Ok(new { message = "User registered successfully", username = user.Username });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _userService.AuthenticateAsync(request.Username, request.Password);
        if (user == null) return Unauthorized("Invalid credentials");

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

        return Ok(new { token = newToken, refreshToken = newRefresh });
    }
}