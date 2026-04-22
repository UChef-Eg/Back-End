using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UcheifBackend.DTOs;
using UcheifBackend.Services;

namespace UcheifBackend.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);

        if (!result.IsSuccess)
        {
            return BadRequest(new { result.Errors });
        }

        return Ok(new { Message = "Registration successful" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);

        if (!result.IsSuccess)
        {
            if (result.IsUnauthorized) return Unauthorized(new { result.Errors });
            return BadRequest(new { result.Errors });
        }

        return Ok(result.Response);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var result = await _authService.RefreshTokenAsync(request);

        if (!result.IsSuccess)
        {
            if (result.IsUnauthorized) return Unauthorized(new { result.Errors });
            return BadRequest(new { result.Errors });
        }

        return Ok(result.Response);
    }

    [HttpPost("revoke-token")]
    [Authorize]
    public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest request)
    {
        var result = await _authService.RevokeTokenAsync(request);

        if (!result.IsSuccess)
        {
            return BadRequest(new { result.Errors });
        }

        return Ok(new { Message = "Token revoked successfully" });
    }
}
