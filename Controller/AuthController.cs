using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using PolicyManagement.Models;
using PolicyManagement.Services;
using PolicyManagement.Helpers;

namespace PolicyManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _config;
    private readonly string _jsonPath;

    public AuthController(IUserService userService, IConfiguration config)
    {
        _userService = userService;
        _config = config;
        _jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "users.json");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = await _userService.AuthenticateAsync(request.Username, request.Password);
        if (user == null) return Unauthorized("Invalid credentials");

        var claims = new List<Claim>
        {
            new Claim("userId", user.UserId),
            new Claim("name", user.Name),
            new Claim("username", user.Username)
        };

        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
        claims.AddRange(user.Permissions.Select(p => new Claim("permission", p)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddHours(1);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        var refreshToken = TokenHelper.GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        var json = System.IO.File.ReadAllText(_jsonPath);
        var users = JsonSerializer.Deserialize<List<User>>(json) ?? new();
        var updateUser = users.FirstOrDefault(u => u.Username == user.Username);

        if (updateUser != null)
        {
            updateUser.RefreshToken = refreshToken;
            updateUser.RefreshTokenExpiryTime = user.RefreshTokenExpiryTime;
        }

        System.IO.File.WriteAllText(_jsonPath, JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true }));

        return Ok(new
        {
            token = tokenString,
            refreshToken = refreshToken,
            expiresAt = expires
        });
    }

    [HttpPost("refresh-token")]
    public IActionResult RefreshToken([FromBody] TokenRequest request)
    {
        var principal = TokenHelper.GetPrincipalFromExpiredToken(request.AccessToken, _config);
        if (principal == null) return BadRequest("Invalid access token");

        var username = principal.Claims.FirstOrDefault(c => c.Type == "username")?.Value;
        if (string.IsNullOrEmpty(username)) return BadRequest("Invalid token");

        var json = System.IO.File.ReadAllText(_jsonPath);
        var users = JsonSerializer.Deserialize<List<User>>(json) ?? new();
        var user = users.FirstOrDefault(u => u.Username == username);

        if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            return Unauthorized("Invalid refresh token");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddHours(1);

        var newToken = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: principal.Claims,
            expires: expires,
            signingCredentials: creds
        );

        var newRefreshToken = TokenHelper.GenerateRefreshToken();
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        System.IO.File.WriteAllText(_jsonPath, JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true }));

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(newToken),
            refreshToken = newRefreshToken,
            expiresAt = expires
        });
    }
}
