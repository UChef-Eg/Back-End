using System.ComponentModel.DataAnnotations;

namespace UcheifBackend.DTOs;

public class RegisterRequest
{
    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password
);

public record GoogleAuthRequest(
    [Required] string IdToken
);

public record RefreshTokenRequest(
    [Required] string Token
);

public record RevokeTokenRequest(
    [Required] string Token
);

public record AuthResponse(
    string Token,
    string RefreshToken,
    UserDto User
);

public record UserDto(
    string Id,
    string FullName,
    string Email,
    List<string> Roles
);

public class AuthResult
{
    public bool IsSuccess { get; set; }
    public bool IsUnauthorized { get; set; }
    public IEnumerable<string> Errors { get; set; } = Enumerable.Empty<string>();
    public AuthResponse? Response { get; set; }
}
