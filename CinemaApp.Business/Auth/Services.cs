using System;
using System.IdentityModel.Tokens.Jwt;
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
            var secret = _cfg["Jwt:SecretKey"] ?? _cfg["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(secret))
                throw new InvalidOperationException("JWT SecretKey is missing in configuration.");

            // Base64 destekle + minimum 32 byte kontrolü
            byte[] keyBytes;
            try { keyBytes = Convert.FromBase64String(secret); }
            catch { keyBytes = Encoding.UTF8.GetBytes(secret); }

            if (keyBytes.Length < 32)
                throw new InvalidOperationException("JWT secret minimum 32 byte (256 bit) olmalı.");

            var creds = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new System.Security.Claims.Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString()),
                new System.Security.Claims.Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var expireMinutes = int.TryParse(_cfg["Jwt:ExpireMinutes"], out var m) ? m : 60;

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
