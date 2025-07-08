using ItemManagementSystem.Application.Interface;
using ItemManagementSystem.Domain.Constants;
using ItemManagementSystem.Domain.DataModels;
using ItemManagementSystem.Domain.Exception;
using ItemManagementSystem.Infrastructure.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ItemManagementSystem.Application.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<User> _userRepo;
        private readonly IConfiguration _configuration;

        public AuthService(IRepository<User> userRepo, IConfiguration configuration)
        {
            _userRepo = userRepo;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            var user = (await _userRepo.FindAsync(u => u.Email == email && u.Active)).FirstOrDefault();
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                throw new NullObjectException(AppMessages.InvalidCredentials);

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
        public string GenerateJwtToken(string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured.");
            var key = Encoding.ASCII.GetBytes(jwtKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, email)
                }),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            var handler = new JwtSecurityTokenHandler();
            ClaimsPrincipal claimsPrincipal;
            var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured.");

            try
            {
                claimsPrincipal = handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(jwtKey))
                }, out SecurityToken validatedToken);
            }
            catch (Exception)
            {
                throw new CustomException(AppMessages.InvalidToken);
            }

            var emailClaim = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email);
            if (emailClaim == null)
            {
                throw new CustomException(AppMessages.EmailClaimNotFound);
            }

            var email = emailClaim.Value;
            var user = (await _userRepo.FindAsync(u => u.Email == email && u.Active)).FirstOrDefault();
            if (user == null)
            {
                throw new NullObjectException(AppMessages.UserNotFound);
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);

            await _userRepo.UpdateAsync(user);

            return true;
        }
    }
}
