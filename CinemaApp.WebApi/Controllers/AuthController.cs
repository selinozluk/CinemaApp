using System.ComponentModel.DataAnnotations;
using CinemaApp.Business.Auth;
using CinemaApp.Data.Context;
using CinemaApp.Data.Entities;
using CinemaApp.WebApi.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaApp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IJwtTokenService _jwt;
    private readonly IDataProtector _protector;
    private readonly CinemaDbContext _db;
    private readonly IPasswordHasher<UserEntity> _hasher;

    public AuthController(
        IJwtTokenService jwt,
        IDataProtectionProvider dp,
        CinemaDbContext db,
        IPasswordHasher<UserEntity> hasher)
    {
        _jwt = jwt;
        _protector = dp.CreateProtector("CinemaApp.AuthController");
        _db = db;
        _hasher = hasher;
    }

    public record RegisterRequest(
        [Required, EmailAddress] string Email,
        [Required, MinLength(6)] string Password,
        [Required] string FirstName,
        [Required] string LastName,
        UserRole Role = UserRole.Customer
    );

    public record LoginRequest(
        [Required, EmailAddress] string Email,
        [Required] string Password
    );

    public record AuthResponse(string Token, string Email, string Role);

    [HttpPost("register")]
    [AllowAnonymous]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req, CancellationToken ct)
    {
        if (await _db.Users.AnyAsync(u => u.Email == req.Email, ct))
            return Conflict(new { message = "Email already exists." });

        var user = new UserEntity
        {
            Email = req.Email.Trim(),
            FirstName = req.FirstName.Trim(),
            LastName = req.LastName.Trim(),
            Role = req.Role
        };
        user.PasswordHash = _hasher.HashPassword(user, req.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        var token = _jwt.CreateToken(user);
        return Created(string.Empty, new AuthResponse(token, user.Email, user.Role.ToString()));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> Login([FromBody] LoginRequest req, CancellationToken ct)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == req.Email, ct);
        if (user is null) return Unauthorized(new { message = "Invalid credentials." });

        var vr = _hasher.VerifyHashedPassword(user, user.PasswordHash, req.Password);
        if (vr == PasswordVerificationResult.Failed)
            return Unauthorized(new { message = "Invalid credentials." });

        var token = _jwt.CreateToken(user);
        return Ok(new AuthResponse(token, user.Email, user.Role.ToString()));
    }

    [HttpGet("protect-demo")]
    [Authorize]
    public IActionResult ProtectDemo([FromQuery] string value)
    {
        var cipher = _protector.Protect(value);
        var plain = _protector.Unprotect(cipher);
        return Ok(new { cipher, plain });
    }
}
