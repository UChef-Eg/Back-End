using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UcheifBackend.Data;
using UcheifBackend.DTOs;
using UcheifBackend.Helpers;
using UcheifBackend.Models;

namespace UcheifBackend.Services;

public class AuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly AppDbContext _context;
    private readonly JwtHelper _jwtHelper;

    public AuthService(UserManager<AppUser> userManager, AppDbContext context, JwtHelper jwtHelper)
    {
        _userManager = userManager;
        _context = context;
        _jwtHelper = jwtHelper;
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        if (await _userManager.FindByEmailAsync(request.Email) != null)
        {
            return new AuthResult { IsSuccess = false, Errors = new[] { "Email is already registered." } };
        }

        var user = new AppUser
        {
            Email = request.Email,
            UserName = request.Email,
            FullName = request.FullName,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return new AuthResult { IsSuccess = false, Errors = result.Errors.Select(e => e.Description) };
        }

        return new AuthResult { IsSuccess = true };
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return new AuthResult { IsSuccess = false, IsUnauthorized = true, Errors = new[] { "Invalid email or password." } };
        }

        if (!user.IsActive)
        {
            return new AuthResult { IsSuccess = false, IsUnauthorized = true, Errors = new[] { "Your account has been deactivated." } };
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtHelper.GenerateToken(user, roles);
        var refreshToken = _jwtHelper.GenerateRefreshToken(user.Id);

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return new AuthResult
        {
            IsSuccess = true,
            Response = new AuthResponse(token, refreshToken.Token, new UserDto(user.Id, user.FullName, user.Email!, roles.ToList()))
        };
    }

    public async Task<AuthResult> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var existingToken = await _context.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == request.Token);

        if (existingToken == null || !existingToken.IsActive)
        {
            return new AuthResult { IsSuccess = false, IsUnauthorized = true, Errors = new[] { "Invalid or expired refresh token." } };
        }

        existingToken.RevokedOn = DateTime.UtcNow;

        var roles = await _userManager.GetRolesAsync(existingToken.User);
        var newToken = _jwtHelper.GenerateToken(existingToken.User, roles);
        var newRefreshToken = _jwtHelper.GenerateRefreshToken(existingToken.UserId);

        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync();

        return new AuthResult
        {
            IsSuccess = true,
            Response = new AuthResponse(newToken, newRefreshToken.Token, new UserDto(existingToken.User.Id, existingToken.User.FullName, existingToken.User.Email!, roles.ToList()))
        };
    }

    public async Task<AuthResult> RevokeTokenAsync(RevokeTokenRequest request)
    {
        var token = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == request.Token);

        if (token == null || !token.IsActive)
        {
            return new AuthResult { IsSuccess = false, Errors = new[] { "Invalid token or already revoked." } };
        }

        token.RevokedOn = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return new AuthResult { IsSuccess = true };
    }
}
