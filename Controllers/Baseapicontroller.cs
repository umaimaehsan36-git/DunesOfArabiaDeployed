// =============================================================
// File: Controllers/BaseApiController.cs
// Path: DunesOfArabia/Controllers/BaseApiController.cs
//
// All your API controllers should inherit from this instead of
// ControllerBase. It:
//   - Applies [Authorize] globally (JWT required by default)
//   - Applies [ApiController] and [Route] conventions
//   - Provides helper methods to get the current user's ID and roles
// =============================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DunesOfArabia.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = "Bearer")]   // ← JWT required on all actions
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    // ── Helpers available in every controller ─────────────────

    /// <summary>Returns the current user's ID from the JWT claim.</summary>
    protected string? CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier);

    /// <summary>Returns the current user's email from the JWT claim.</summary>
    protected string? CurrentUserEmail =>
        User.FindFirstValue(ClaimTypes.Email);

    /// <summary>True if the current user has the Admin role.</summary>
    protected bool IsAdmin =>
        User.IsInRole("Admin");

    /// <summary>
    /// Returns 401 if the calling user's ID doesn't match <paramref name="resourceOwnerId"/>,
    /// unless the caller is an Admin.
    /// Use this to prevent users from accessing each other's data.
    /// </summary>
    protected IActionResult? ForbidIfNotOwner(string resourceOwnerId)
    {
        if (CurrentUserId != resourceOwnerId && !IsAdmin)
            return Forbid();
        return null;
    }
}