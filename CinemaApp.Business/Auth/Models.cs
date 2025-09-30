using System.ComponentModel.DataAnnotations;

namespace CinemaApp.Business.Auth;

public class RegisterRequest
{
    [Required, EmailAddress] public string Email { get; set; } = null!;
    [Required, MinLength(6)] public string Password { get; set; } = null!;
    [Required] public string FirstName { get; set; } = null!;
    [Required] public string LastName { get; set; } = null!;
}

public class LoginRequest
{
    [Required, EmailAddress] public string Email { get; set; } = null!;
    [Required] public string Password { get; set; } = null!;
}

public class AuthResponse
{
    public bool Succeeded { get; set; }
    public string? Message { get; set; }
    public string? Token { get; set; } // jwt
}
