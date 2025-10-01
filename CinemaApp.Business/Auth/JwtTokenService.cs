using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using CinemaApp.Data.Entities;

namespace CinemaApp.Business.Auth
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _cfg;
        public JwtTokenService(IConfiguration cfg) => _cfg = cfg;

        public string CreateToken(UserEntity user)
        {
            var issuer = _cfg["Jwt:Issuer"] ?? "CinemaApp";
            var audience = _cfg["Jwt:Audience"] ?? "CinemaApp.Client";
            var secret = _cfg["Jwt:SecretKey"] ?? _cfg["Jwt:Key"]
                           ?? throw new InvalidOperationException("JWT SecretKey is missing.");

            // Base64 ya da düz string destekle + min 32 byte
            byte[] keyBytes;
            try { keyBytes = Convert.FromBase64String(secret); }
            catch { keyBytes = Encoding.UTF8.GetBytes(secret); }
            if (keyBytes.Length < 32)
                throw new InvalidOperationException("JWT secret minimum 32 byte (256 bit) olmalı.");

            var creds = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email ?? string.Empty),
                new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}".Trim()),
                new(ClaimTypes.Role, user.Role.ToString()), 
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var minutes = int.TryParse(_cfg["Jwt:ExpireMinutes"], out var m) ? m : 60;

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(minutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
