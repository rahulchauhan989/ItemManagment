using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ItemManagementSystem.Application.Interface;
using ItemManagementSystem.Domain.DataModels;
using ItemManagementSystem.Infrastructure.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ItemManagementSystem.Application.Implementation;

public class AuthService : IAuthService
{
    private readonly IRepository<User> _userRepo;
    private readonly string _jwtSecret;
    private readonly TimeSpan _jwtLifetime;
    private readonly IConfiguration _configuration;


    public AuthService(IRepository<User> userRepo, IConfiguration config, IConfiguration configuration)
    {
        _userRepo = userRepo;
        _jwtSecret = config["Jwt:Secret"];
        _jwtLifetime = TimeSpan.FromMinutes(Convert.ToInt32(config["Jwt:LifetimeMinutes"]));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<string> LoginAsync(string email, string password)
    {
        var user = (await _userRepo.FindAsync(u => u.Email == email && u.Active)).FirstOrDefault();
        // if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
        //     throw new UnauthorizedAccessException("Invalid credentials");

        // if(user!=null || user.Password != password)
        //     throw new UnauthorizedAccessException("Invalid credentials");

        var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured.");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        string role = "unknown";

        if (user.RoleId == 1)
        {
            role = "Admin";
        }
        else 
        {
            role = "User";
        }

        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, role)
    };

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);

        string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        var handler = new JwtSecurityTokenHandler();
        var decodedToken = handler.ReadJwtToken(tokenString);

        Console.WriteLine(" Decoded Token Claims:");
        foreach (var claim in decodedToken.Claims)
        {
            Console.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");
        }
        return tokenString;

    }

    public async Task ForgotPasswordAsync(string email)
    {

    }

    public async Task ResetPasswordAsync(string token, string newPassword)
    {

    }
}
