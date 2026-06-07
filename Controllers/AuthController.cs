// =============================================================
// File: Controllers/AuthController.cs
// Path: DunesOfArabia/Controllers/AuthController.cs
// =============================================================

using DunesOfArabia.Data;
using DunesOfArabia.Models;
using DunesOfArabia.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DunesOfArabia.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtService jwtService,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
        _logger = logger;
    }

    // ── POST /api/auth/register ───────────────────────────────
    /// <summary>Register a new user and return a JWT token.</summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                message = "Registration failed",
                errors = result.Errors.Select(e => e.Description)
            });
        }

        // Assign default "User" role
        await _userManager.AddToRoleAsync(user, "User");

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtService.GenerateToken(user, roles);

        _logger.LogInformation("New user registered: {Email}", user.Email);

        return Ok(new AuthResponse
        {
            Token = token,
            UserId = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            Roles = roles.ToList(),
            Message = "Registration successful"
        });
    }

    // ── POST /api/auth/login ──────────────────────────────────
    /// <summary>Login with email + password and return a JWT token.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Unauthorized(new { message = "Invalid email or password" });

        var result = await _signInManager.CheckPasswordSignInAsync(
            user, request.Password, lockoutOnFailure: true);

        if (result.IsLockedOut)
            return Unauthorized(new { message = "Account locked. Please try again later." });

        if (!result.Succeeded)
            return Unauthorized(new { message = "Invalid email or password" });

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtService.GenerateToken(user, roles);

        _logger.LogInformation("User logged in via API: {Email}", user.Email);

        return Ok(new AuthResponse
        {
            Token = token,
            UserId = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            Roles = roles.ToList(),
            Message = "Login successful"
        });
    }

    // ── GET /api/auth/me ──────────────────────────────────────
    /// <summary>Returns the current user's profile. Requires valid JWT.</summary>
    [HttpGet("me")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> Me()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId is null) return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return NotFound();

        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new
        {
            userId = user.Id,
            email = user.Email,
            fullName = user.FullName,
            roles = roles
        });
    }

    // ── Request / Response models ─────────────────────────────

    public record RegisterRequest(
        [Required][StringLength(100)] string FullName,
        [Required][EmailAddress] string Email,
        [Required][StringLength(100, MinimumLength = 6)] string Password
    );

    public record LoginRequest(
        [Required][EmailAddress] string Email,
        [Required] string Password
    );

    public class AuthResponse
    {
        public string Token { get; set; } = "";
        public string UserId { get; set; } = "";
        public string Email { get; set; } = "";
        public string? FullName { get; set; }
        public List<string> Roles { get; set; } = new();
        public string Message { get; set; } = "";
    }
}