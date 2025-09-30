using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
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
            var secret = _cfg["Jwt:SecretKey"] ?? _cfg["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(secret))
                throw new InvalidOperationException("JWT SecretKey is missing in configuration.");

            byte[] keyBytes;
            try { keyBytes = Convert.FromBase64String(secret); }
            catch { keyBytes = Encoding.UTF8.GetBytes(secret); }
            if (keyBytes.Length < 32)
                throw new InvalidOperationException("JWT secret minimum 32 byte (256 bit) olmalı.");

            var creds = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var expireMinutes = int.TryParse(_cfg["Jwt:ExpireMinutes"], out var m) ? m : 60;

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}