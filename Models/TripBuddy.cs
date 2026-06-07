// DunesOfArabia/Models/TripBuddy.cs
// ─────────────────────────────────────────────────────────────────────────────
// Three models that back the TripBuddy.razor co-traveler matching feature.
//
// RELATIONSHIPS:
//   TripBuddyPost         ← one user posts a trip
//   TripBuddyJoinRequest  ← another user requests to join that post
//   TripBuddyChatMessage  ← either user sends messages on that post thread
//
// ApplicationDbContext registers:
//   public DbSet<TripBuddyPost>        TripBuddyPosts        => Set<TripBuddyPost>();
//   public DbSet<TripBuddyChatMessage> TripBuddyMessages     => Set<TripBuddyChatMessage>();
//   public DbSet<TripBuddyJoinRequest> TripBuddyJoinRequests => Set<TripBuddyJoinRequest>();
//
// MIGRATION:
//   Add-Migration AddTripBuddyRecipientId   ← adds RecipientId column to TripBuddyMessages
//   Update-Database
//
// NOTE: If you already ran AddTripBuddy, run a NEW migration:
//   Add-Migration AddTripBuddyChatRecipientId
//   Update-Database
// ─────────────────────────────────────────────────────────────────────────────

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DunesOfArabia.Models
{
    // ══════════════════════════════════════════════════════════════════════════
    //  TRIP BUDDY POST
    //  A user announces they're planning a trip and invites co-travelers.
    // ══════════════════════════════════════════════════════════════════════════

    public class TripBuddyPost
    {
        public int Id { get; set; }

        /// <summary>FK to AspNetUsers.Id — the user who posted the trip.</summary>
        [Required]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Display name shown on the card. Denormalized from ApplicationUser.FullName
        /// so it renders without a join when loading the feed.
        /// </summary>
        [MaxLength(120)]
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>FK to Destinations.Id</summary>
        public int DestinationId { get; set; }

        /// <summary>
        /// Denormalized destination name — shown on cards without a join.
        /// Also used by the filter chips in TripBuddy.razor.
        /// </summary>
        [MaxLength(120)]
        public string DestinationName { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }
        public DateTime EndDate   { get; set; }

        /// <summary>Adventure | Cultural | Relaxation | Family | Business</summary>
        [MaxLength(30)]
        public string TripType { get; set; } = "Adventure";

        /// <summary>e.g. "Mid-range (SR 1500–4000)"</summary>
        [MaxLength(60)]
        public string BudgetRange { get; set; } = string.Empty;

        /// <summary>Free-text intro shown on the card. Max 400 chars.</summary>
        [MaxLength(400)]
        public string Bio { get; set; } = string.Empty;

        /// <summary>Total number of spots in the group (including the poster).</summary>
        public int TotalSpots { get; set; } = 2;

        /// <summary>
        /// Remaining open spots. Decremented when a JoinRequest is Accepted.
        /// TripBuddy.razor shows "🟢 Open" when SpotsLeft > 0.
        /// </summary>
        public int SpotsLeft { get; set; } = 1;

        /// <summary>Computed — no DB column needed.</summary>
        [NotMapped]
        public bool IsOpen => SpotsLeft > 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ── Navigation ────────────────────────────────────────────────────────
        [NotMapped]
        public ApplicationUser? User { get; set; }

        [NotMapped]
        public Destination? Destination { get; set; }
    }


    // ══════════════════════════════════════════════════════════════════════════
    //  TRIP BUDDY CHAT MESSAGE
    //  Any message sent in the thread associated with a TripBuddyPost.
    //  Both the poster and any requester can send messages.
    // ══════════════════════════════════════════════════════════════════════════

    public class TripBuddyChatMessage
    {
        public int Id { get; set; }

        /// <summary>FK to TripBuddyPosts.Id</summary>
        public int PostId { get; set; }

        /// <summary>FK to AspNetUsers.Id — who sent the message.</summary>
        [Required]
        public string SenderId { get; set; } = string.Empty;

        /// <summary>
        /// Denormalized sender name so the bubble renders without a join.
        /// TripBuddy.razor uses this to decide "mine" vs "theirs" label.
        /// </summary>
        [MaxLength(120)]
        public string SenderName { get; set; } = string.Empty;

        /// <summary>
        /// FK to AspNetUsers.Id — the intended recipient of this message.
        /// Required so that messages between different pairs of users on the same
        /// post are never mixed up. Always stored as the *other* party:
        ///   visitor  → owner  : RecipientId = post.UserId
        ///   owner    → visitor: RecipientId = requester's UserId
        /// </summary>
        [Required]
        [MaxLength(450)]
        public string RecipientId { get; set; } = string.Empty;

        /// <summary>Message body — no hard limit enforced at model level.</summary>
        public string Text { get; set; } = string.Empty;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }


    // ══════════════════════════════════════════════════════════════════════════
    //  TRIP BUDDY JOIN REQUEST
    //  Sent when a user clicks "🤝 Request to Join" on a TripBuddyPost.
    //  Opening the chat auto-sends a request. The post owner can Accept/Reject.
    //  Accepting decrements TripBuddyPost.SpotsLeft.
    // ══════════════════════════════════════════════════════════════════════════

    public class TripBuddyJoinRequest
    {
        public int Id { get; set; }

        /// <summary>FK to TripBuddyPosts.Id</summary>
        public int PostId { get; set; }

        /// <summary>FK to AspNetUsers.Id — the user who wants to join.</summary>
        [Required]
        public string RequesterId { get; set; } = string.Empty;

        /// <summary>
        /// Denormalized display name of the requester — stored at request-send time
        /// so the owner's inbox can show it without an extra join.
        /// </summary>
        [MaxLength(120)]
        public string RequesterName { get; set; } = string.Empty;

        /// <summary>Pending | Accepted | Rejected</summary>
        [MaxLength(20)]
        public string Status { get; set; } = "Pending";

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

        // ── Navigation ────────────────────────────────────────────────────────
        [NotMapped]
        public TripBuddyPost? Post { get; set; }

        [NotMapped]
        public ApplicationUser? Requester { get; set; }
    }
}
