using Microsoft.AspNetCore.Identity;
using DunesOfArabia.Models;

namespace DunesOfArabia.Services
{
    public class RegisterRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResult
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? Error { get; set; }
        public string? UserName { get; set; }
        public string? Role { get; set; }
    }

    public interface IAuthService
    {
        Task<AuthResult> RegisterAsync(RegisterRequest request, string role = "User");
        Task<AuthResult> LoginAsync(LoginRequest request);
    }

    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtService _jwtService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IJwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
        }

        public async Task<AuthResult> RegisterAsync(RegisterRequest req, string role = "User")
        {
            if (await _userManager.FindByEmailAsync(req.Email) != null)
                return new AuthResult { Success = false, Error = "Email already registered." };

            var user = new ApplicationUser
            {
                UserName = req.Email,
                Email = req.Email,
                FullName = req.FullName
            };

            var result = await _userManager.CreateAsync(user, req.Password);
            if (!result.Succeeded)
                return new AuthResult { Success = false, Error = string.Join(", ", result.Errors.Select(e => e.Description)) };

            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            await _userManager.AddToRoleAsync(user, role);

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtService.GenerateToken(user, roles);

            return new AuthResult { Success = true, Token = token, UserName = user.FullName ?? user.Email, Role = role };
        }

        public async Task<AuthResult> LoginAsync(LoginRequest req)
        {
            var user = await _userManager.FindByEmailAsync(req.Email);
            if (user == null)
                return new AuthResult { Success = false, Error = "Invalid email or password." };

            var result = await _signInManager.CheckPasswordSignInAsync(user, req.Password, false);
            if (!result.Succeeded)
                return new AuthResult { Success = false, Error = "Invalid email or password." };

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtService.GenerateToken(user, roles);

            return new AuthResult { Success = true, Token = token, UserName = user.FullName ?? user.Email, Role = roles.FirstOrDefault() ?? "User" };
        }
    }
}