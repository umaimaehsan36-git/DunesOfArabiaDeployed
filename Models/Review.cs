namespace DunesOfArabia.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int? DestinationId { get; set; }  // FIX: int -> int? so that ?? operator compiles in Profile.razor line 470
        public int? ActivityId { get; set; }

        // ── StarRating kept for service; Rating alias for Razor components ──
        public int StarRating { get; set; }
        public double Rating => StarRating;

        // ── Comment kept for service; Text alias for Razor components ────────
        public string Comment { get; set; } = string.Empty;
        public string Text => Comment;

        // ── CreatedAt kept for service; CreatedDate alias for Razor components ─
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // ── UserName — resolved from navigation property ──────────────────────
        public string UserName => User?.FullName ?? User?.UserName ?? UserId;

        // ── Navigation ────────────────────────────────────────────────────────
        public Destination? Destination { get; set; }
        public ApplicationUser? User { get; set; }
    }
}