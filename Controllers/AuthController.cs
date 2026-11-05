
using CampusPlacementManagement.Data;
using CampusPlacementManagement.DTOs.Auth;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Models.Enums;
using CampusPlacementManagement.Services;
using CampusPlacementManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _configuration;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context,
        IJwtService jwtService,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _context = context;
        _jwtService = jwtService;
        _configuration = configuration;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null ||
            !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        if (user.AccountStatus != AccountStatus.Approved)
        {
            return Unauthorized(new { message = "Your account is not active or approved." });
        }

        var roles = await _userManager.GetRolesAsync(user);

        if (!await IsProfileApproved(user, roles))
        {
            return Unauthorized(new { message = "Your profile is not approved." });
        }

        var accessToken = _jwtService.GenerateAccessToken(user, roles);

        var refreshToken = TokenHelper.GenerateRefreshToken();

        var refreshTokenDays = Convert.ToDouble(
            _configuration["Jwt:RefreshTokenDays"]);

        _context.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = TokenHelper.HashToken(refreshToken),
            ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenDays)
        });

        await _context.SaveChangesAsync();

        return Ok(new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(
                Convert.ToDouble(_configuration["Jwt:AccessTokenMinutes"]))
        });
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(
        RefreshTokenRequest request)
    {
        var tokenHash = TokenHelper.HashToken(request.RefreshToken);

        var storedToken = await _context.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.TokenHash == tokenHash);

        if (storedToken == null ||
            storedToken.RevokedAt != null ||
            storedToken.ExpiresAt <= DateTime.UtcNow)
        {
            return Unauthorized(new { message = "Invalid or expired refresh token." });
        }

        var user = storedToken.User;

        if (user.AccountStatus != AccountStatus.Approved)
        {
            return Unauthorized(new { message = "Your account is not active or approved." });
        }

        var roles = await _userManager.GetRolesAsync(user);

        if (!await IsProfileApproved(user, roles))
        {
            return Unauthorized(new { message = "Your profile is not approved." });
        }

        var newRefreshToken = TokenHelper.GenerateRefreshToken();
        var newRefreshTokenHash = TokenHelper.HashToken(newRefreshToken);

        storedToken.RevokedAt = DateTime.UtcNow;
        storedToken.ReplacedByTokenHash = newRefreshTokenHash;

        _context.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = newRefreshTokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(
                Convert.ToDouble(_configuration["Jwt:RefreshTokenDays"]))
        });

        await _context.SaveChangesAsync();

        return Ok(new AuthResponse
        {
            AccessToken = _jwtService.GenerateAccessToken(user, roles),
            RefreshToken = newRefreshToken,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(
                Convert.ToDouble(_configuration["Jwt:AccessTokenMinutes"]))
        });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = _userManager.GetUserId(User);

        var user = await _userManager.FindByIdAsync(userId!);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            user.Id,
            user.FullName,
            user.Email,
            user.PhoneNumber,
            user.AccountStatus,
            Roles = await _userManager.GetRolesAsync(user)
        });
    }

    private async Task<bool> IsProfileApproved(
        ApplicationUser user,
        IList<string> roles)
    {
        if (roles.Contains("PlacementOfficer"))
        {
            return await _context.PlacementOfficers.AnyAsync(p =>
                p.UserId == user.Id &&
                p.Status == ApprovalStatus.Approved);
        }

        if (roles.Contains("CompanyHR"))
        {
            return await _context.CompanyHRs.AnyAsync(h =>
                h.UserId == user.Id &&
                h.Status == ApprovalStatus.Approved);
        }

        if (roles.Contains("Student"))
        {
            return await _context.Students.AnyAsync(s =>
                s.UserId == user.Id &&
                s.Status == ApprovalStatus.Approved);
        }

        return roles.Contains("SuperAdmin");
    }
}
