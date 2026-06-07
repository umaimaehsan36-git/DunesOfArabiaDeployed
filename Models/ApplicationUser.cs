using Microsoft.AspNetCore.Identity;

namespace DunesOfArabia.Models
{
    /// <summary>
    /// Extended Identity user for Saudi Heritage / Dunes of Arabia.
    ///
    /// CHANGES FROM ORIGINAL:
    ///   1. Added CreatedAt — required by Profile.razor ("Member since @memberSince"),
    ///      by AdminDashboard.razor (JoinedAt column), and by the role-seeding admin seed.
    ///      Set automatically in the constructor so it is never null.
    ///   2. Added AvatarUrl — Profile.razor avatar upload stores the URL here.
    ///   3. Added FirstName / LastName — Register.razor now captures them separately.
    ///      FullName is kept for backward-compatibility (existing queries / Profile banner).
    ///   4. Kept PhoneNumber inherited from IdentityUser (no need to re-declare).
    ///      ApplicationDbContext already marks it IsRequired(false) — no change needed.
    ///
    /// MIGRATION REQUIRED after adding these properties:
    ///   dotnet ef migrations add AddUserProfileFields
    ///   dotnet ef database update
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        // ── Name ──────────────────────────────────────────────────────────────
        /// <summary>Full display name (e.g. "Khalid Al-Harbi").</summary>
        public string? FullName { get; set; }

        /// <summary>First name — captured on registration.</summary>
        public string? FirstName { get; set; }

        /// <summary>Last name — captured on registration.</summary>
        public string? LastName { get; set; }

        // ── Profile ───────────────────────────────────────────────────────────
        /// <summary>
        /// URL of the user's uploaded avatar image.
        /// Null means no custom avatar — UI should fall back to initials.
        /// </summary>
        public string? AvatarUrl { get; set; }

        // ── Audit ─────────────────────────────────────────────────────────────
        /// <summary>
        /// UTC timestamp when the account was created.
        /// Defaults to UtcNow in the constructor — never null at runtime.
        /// Used by Profile.razor ("Member since"), AdminDashboard.razor (JoinedAt),
        /// and the role-seeding admin seed in Program.cs.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        // ── Constructor ───────────────────────────────────────────────────────
        public ApplicationUser()
        {
            CreatedAt = DateTime.UtcNow;
        }
    }
}