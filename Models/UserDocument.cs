// DunesOfArabia/Models/UserDocument.cs
// ─────────────────────────────────────────────────────────────────────────────
// Identity-verification document tied to a user account.
//
// WHY THIS EXISTS:
//   Profile.razor and Checkout.razor both enforce a 6-point booking gate:
//     Name · Phone · Nationality · Passport · National ID · Selfie
//
//   The last three are checked by reading UserDocument rows filtered by
//   the Category field:
//     docs.Any(d => d.Category == "Passport")
//     docs.Any(d => d.Category == "National ID")
//     docs.Any(d => d.Category == "Selfie")
//
//   This is SEPARATE from BookingDocument (files attached to a single booking).
//   UserDocument is attached to the user account and persists across all bookings.
//
// MIGRATION:
//   Add-Migration AddUserDocument
//   Update-Database
// ─────────────────────────────────────────────────────────────────────────────

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DunesOfArabia.Models
{
    public class UserDocument
    {
        public int Id { get; set; }

        /// <summary>FK to AspNetUsers.Id</summary>
        [Required]
        public string UserId { get; set; } = string.Empty;

        /// <summary>Original filename e.g. "passport_scan.pdf"</summary>
        [MaxLength(200)]
        public string FileName { get; set; } = string.Empty;

        /// <summary>Uppercase extension without dot: "PDF", "JPG", "PNG"</summary>
        [MaxLength(10)]
        public string FileType { get; set; } = string.Empty;

        /// <summary>
        /// Booking-gate category.
        /// Profile.razor lets the user assign this after upload.
        /// Valid values: Passport | National ID | Selfie | Visa | Ticket | Other
        /// The booking gate in Profile.razor and Checkout.razor checks for
        /// the first three specifically.
        /// </summary>
        [MaxLength(30)]
        public string Category { get; set; } = "Other";

        /// <summary>File size in bytes — shown in the Documents tab UI.</summary>
        public long FileSizeBytes { get; set; }

        /// <summary>
        /// Relative URL served by the app e.g. "/uploads/{userId}/guid_filename.pdf"
        /// Null until the upload completes successfully.
        /// </summary>
        [MaxLength(500)]
        public string? FileUrl { get; set; }

        /// <summary>UTC timestamp when the document was uploaded.</summary>
        public DateTime UploadedOn { get; set; } = DateTime.UtcNow;

        // ── Navigation ────────────────────────────────────────────────────────
        [NotMapped]
        public ApplicationUser? User { get; set; }
    }
}
