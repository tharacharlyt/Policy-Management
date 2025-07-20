using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using PolicyManagement.Models;
using PolicyManagement.Services;
using System.ComponentModel.DataAnnotations;

namespace PolicyManagement.Controllers
{
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

        // ✅ LOGIN
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

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }

        // ✅ REGISTER NEW USER
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var json = System.IO.File.ReadAllText(_jsonPath);
            var users = JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();

            if (users.Any(u => u.UserId == request.UserId))
                return Conflict("UserId already exists");

            if (users.Any(u => u.Username == request.Username))
                return Conflict("Username already exists");

            var newUser = new User
            {
                UserId = request.UserId,
                Name = request.Name,
                Username = request.Username,
                Password = request.Password,
                Roles = new List<string>(),
                Permissions = new List<string>()
            };

            users.Add(newUser);
            System.IO.File.WriteAllText(_jsonPath, JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true }));

            return Ok("User registered successfully");
        }

        // ✅ ADD ROLES AND PERMISSIONS TO USER
        [HttpPost("add-roles-permissions")]
        public IActionResult AddRolesPermissions([FromBody] RolePermissionRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var json = System.IO.File.ReadAllText(_jsonPath);
            var users = JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();

            var user = users.FirstOrDefault(u => u.Username == request.Username);
            if (user == null) return NotFound("User not found");

            user.Roles = request.Roles;
            user.Permissions = request.Permissions;

            System.IO.File.WriteAllText(_jsonPath, JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true }));

            return Ok("Roles and permissions updated successfully");
        }
    }
}