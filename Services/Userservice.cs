using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DunesOfArabia.Data;
using DunesOfArabia.Models;

namespace DunesOfArabia.Services
{
    // ═══════════════════════════════════════════════════════
    // USER SERVICE
    // ═══════════════════════════════════════════════════════

    public interface IUserService
    {
        Task<ApplicationUser?> GetByIdAsync(string userId);
        Task<List<ApplicationUser>> GetAllAsync();
        Task<bool> UpdateProfileAsync(string userId, UpdateProfileDto dto);
        Task<bool> DeleteAsync(string userId);
        Task<string?> GetRoleAsync(string userId);
        Task<bool> ChangeRoleAsync(string userId, string newRole);
    }

    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ApplicationUser?> GetByIdAsync(string userId)
            => await _userManager.FindByIdAsync(userId);

        public async Task<List<ApplicationUser>> GetAllAsync()
            => await _userManager.Users.ToListAsync();

        public async Task<bool> UpdateProfileAsync(string userId, UpdateProfileDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return false;

            if (dto.FullName is not null) user.FullName = dto.FullName;
            if (dto.Email is not null)
            {
                user.Email = dto.Email;
                user.UserName = dto.Email;
            }

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> DeleteAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return false;
            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<string?> GetRoleAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return null;
            var roles = await _userManager.GetRolesAsync(user);
            return roles.FirstOrDefault();
        }

        public async Task<bool> ChangeRoleAsync(string userId, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return false;

            // Ensure role exists
            if (!await _roleManager.RoleExistsAsync(newRole))
                await _roleManager.CreateAsync(new IdentityRole(newRole));

            // Remove all current roles, then assign new one
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            var result = await _userManager.AddToRoleAsync(user, newRole);
            return result.Succeeded;
        }
    }

    public record UpdateProfileDto(
        string? FullName,
        string? Email
    );
}