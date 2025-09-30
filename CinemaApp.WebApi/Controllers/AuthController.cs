using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CinemaApp.Business.Auth;
using CinemaApp.Data.Context;
using CinemaApp.Data.Entities;
using CinemaApp.WebApi.Filters;

namespace CinemaApp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IJwtTokenService _jwt;
    private readonly IDataProtector _protector;
    private readonly CinemaDbContext _db; // DB erişimi

    public AuthController(IJwtTokenService jwt, IDataProtectionProvider dp, CinemaDbContext db)
    {
        _jwt = jwt;
        _protector = dp.CreateProtector("CinemaApp.AuthController");
        _db = db;
    }

    // REGISTER (örnek/iskelet)
    [HttpPost("register")]
    [AllowAnonymous]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req, CancellationToken ct)
    {
        // TODO: kendi UserEntity şemanı kullan
        // Örn: Email benzersiz kontrolü
        var exists = await _db.Set<UserEntity>()
                              .AnyAsync(u => u.Email == req.Email, ct);
        if (exists) return Conflict(new { message = "Email already exists." });

        var user = new UserEntity
        {
            Email = req.Email!.Trim()
            // PasswordHash/Salt alanların varsa burada ata
        };

        _db.Add(user);
        await _db.SaveChangesAsync(ct);

        return Created(string.Empty, new { message = "User registered." });
    }

    // LOGIN (JWT üretimi) – DB kontrolünü burada yap
    [HttpPost("login")]
    [AllowAnonymous]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> Login([FromBody] LoginRequest req, CancellationToken ct)
    {
        // 1) Kullanıcıyı getir
        var user = await _db.Set<UserEntity>()
                            .FirstOrDefaultAsync(u => u.Email == req.Email, ct);

        if (user is null)
            return Unauthorized(new { message = "Invalid credentials." });

        // 2) Şifre doğrulaması (şemana göre değiştir)
        // Eğer PasswordHash alanın varsa burada doğrula.
        // Şimdilik demo: şifre kontrolü yapmadan geç
        // if (!PasswordHasher.Verify(req.Password, user.PasswordHash, user.PasswordSalt)) return Unauthorized(...);

        // 3) Token oluştur
        var token = _jwt.CreateToken(user);
        return Ok(new { access_token = token });
    }

    // Korumalı örnek
    [HttpGet("protect-demo")]
    [Authorize]
    public IActionResult ProtectDemo([FromQuery] string value)
    {
        var cipher = _protector.Protect(value);
        var plain = _protector.Unprotect(cipher);
        return Ok(new { cipher, plain });
    }
}

public class RegisterRequest
{
    [Required, EmailAddress] public string Email { get; set; } = default!;
    [Required, MinLength(6)] public string Password { get; set; } = default!;
}

public class LoginRequest
{
    [Required, EmailAddress] public string Email { get; set; } = default!;
    [Required] public string Password { get; set; } = default!;
}