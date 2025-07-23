using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using PolicyManagement.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using PolicyManagement.Validators;

var builder = WebApplication.CreateBuilder(args);

// 🔐 JWT Permissions to configure policies
string[] permissions = new[]
{
    "Policy.Add", "Policy.Edit", "Policy.Delete", "Policy.List",
    "Document.Add", "Document.List"
};

// ✅ Add Controllers
builder.Services.AddControllers();

// ✅ FluentValidation (latest)
builder.Services
    .AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters();

builder.Services.AddValidatorsFromAssemblyContaining<CreatePolicyDtoValidator>();

// ✅ Add Swagger for testing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Add Authorization Policies
builder.Services.AddAuthorization(options =>
{
    foreach (var permission in permissions)
    {
        options.AddPolicy(permission, policy =>
            policy.RequireClaim("permission", permission));
    }
});

// ✅ JWT Authentication configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new Exception("Issuer missing"),
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? throw new Exception("Audience missing"),
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new Exception("Key missing")))
        };
    });

// ✅ Register custom user service
builder.Services.AddSingleton<IUserService, JsonUserService>();

var app = builder.Build();

// ✅ Swagger UI middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ✅ Middleware pipeline
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
